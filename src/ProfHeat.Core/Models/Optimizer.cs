// Copyright 2024 SoftFuzz
//
// Licensed under the Apache License, Version 2.0 (the "License"):
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace ProfHeat.Core.Models;

public static class Optimizer
{
    public static List<OptimizationResult> Optimize(HeatingGrid grid, List<MarketCondition> MarketConditions)
    {
        var optimizationResults = new List<OptimizationResult>();
        var heatDissipationFactor = CalculateHeatDissipation(grid.Buildings);

        foreach (var condition in MarketConditions)
        {
            var adjustedHeatDemand = condition.HeatDemand + heatDissipationFactor;
            var shutdownCostFactor = CalculateShutdownCostFactor(grid.ProductionUnits, adjustedHeatDemand, condition.ElectricityPrice);

            foreach (var unit in OrderUnitsByProfitPotential(grid.ProductionUnits, condition.ElectricityPrice, shutdownCostFactor))
            {
                if (adjustedHeatDemand <= 0)
                {
                    break;
                }

                var productionAmount = Math.Min(unit.MaxHeat, adjustedHeatDemand);
                var cost = productionAmount * unit.ProductionCost;
                var co2Emissions = productionAmount * unit.CO2Emissions;
                var primaryEnergyConsumption = productionAmount * unit.GasConsumption;
                var electricityProduced = unit.MaxElectricity > 0 ? productionAmount * (unit.MaxElectricity / unit.MaxHeat) : 0;

                optimizationResults.Add(new OptimizationResult
                {
                    TimeFrom = condition.TimeFrom,
                    TimeTo = condition.TimeTo,
                    ProducedHeat = productionAmount,
                    ElectricityProduced = electricityProduced,
                    PrimaryEnergyConsumption = primaryEnergyConsumption,
                    Costs = cost,
                    CO2Emissions = co2Emissions
                });

                adjustedHeatDemand -= productionAmount;
            }
        }

        return optimizationResults;
    }

    // Calculate heat dissipation based on building count and average distance.
    // Assumes linear heat loss over distance. (For a simplified calculation)
    private static double CalculateHeatDissipation(int buildings)
    {
        const double HeatLossRate = 0.02;                           // 2% per kilometer
        const double AverageBuildingLength = 10;                    // meters
        const double AverageDistance = 750;                         // meters to the average building

        var totalLength = buildings * AverageBuildingLength;        // Total length of buildings served
        const double dissipationPerMeter = HeatLossRate / 1000;     // Converting loss rate to per meter

        return dissipationPerMeter * AverageDistance * totalLength; // Total dissipation based on distance and building length
    }

    private static double CalculateShutdownCostFactor(List<ProductionUnit> units, double demand, double electricityPrice)
    {
        const double BaseShutdownCost = 500; // Base cost for shutting down a unit
        var totalAvailableCapacity = units.Sum(unit => unit.MaxHeat);
        var shortageRatio = demand / totalAvailableCapacity;

        // Additional factors
        var rampDownCost = units.Sum(unit => unit.MaxHeat * 0.05);                                   // 5% of max heat capacity as ramp down cost
        var efficiencyPenalty = units.Sum(unit => (1 - (unit.MaxElectricity / unit.MaxHeat)) * 100); // Efficiency penalty based on electrical output to heat ratio

        // Market penalty for supply-demand mismatch
        var marketPenalty = shortageRatio > 1 ? (shortageRatio - 1) * electricityPrice * 100 : 0;    // Penalty increases with the ratio exceeding 1

        return BaseShutdownCost + rampDownCost + efficiencyPenalty + marketPenalty;
    }

    private static IEnumerable<ProductionUnit> OrderUnitsByProfitPotential(List<ProductionUnit> units, double electricityPrice, double shutdownCostFactor)
    {
        return units.Select(unit => new
        {
            Unit = unit,
            ProfitPotential = unit.MaxElectricity >= 0
                              ? ((electricityPrice - unit.ProductionCost) * Math.Max(unit.MaxElectricity, unit.MaxHeat)) - shutdownCostFactor
                              : (unit.MaxHeat * unit.ProductionCost) - shutdownCostFactor  // Consider heat production for non-electricity producing units
        })
        .Where(a => a.ProfitPotential > -shutdownCostFactor)
        .OrderByDescending(a => a.ProfitPotential)
        .Select(a => a.Unit);
    }
}
