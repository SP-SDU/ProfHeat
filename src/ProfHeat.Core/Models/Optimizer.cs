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

using ProfHeat.Core.Interfaces;

namespace ProfHeat.Core.Models;

public class Optimizer : IOptimizer
{
    #region Optimizer
    /// <summary> Main optimization method that calculates the optimal costs based on selected production units and market conditions. </summary>
    public List<OptimizationResult> Optimize(HeatingGrid grid, List<MarketCondition> MarketConditions)
    {
        var optimizationResults = new List<OptimizationResult>();
        var heatDissipationFactor = CalculateHeatDissipation(grid.Buildings);

        foreach (var condition in MarketConditions)
        {
            var adjustedHeatDemand = condition.HeatDemand + heatDissipationFactor;
            var shutdownCostFactor = CalculateShutdownCostFactor(grid.ProductionUnits, adjustedHeatDemand, condition.ElectricityPrice);

            foreach (var unit in OrderByCostEffectiveness(grid.ProductionUnits, condition.ElectricityPrice, shutdownCostFactor))
            {
                if (adjustedHeatDemand <= 0)
                {
                    break;
                }

                var productionAmount = Math.Min(unit.MaxHeat, adjustedHeatDemand);
                var cost = productionAmount * unit.ProductionCost;
                var co2Emissions = productionAmount * unit.CO2Emissions;
                var primaryEnergyConsumption = productionAmount * unit.GasConsumption;
                var electricityProduced = (unit.MaxElectricity / unit.MaxHeat) * productionAmount;  // Negative is consumption and Positive is production
                cost -= electricityProduced * condition.ElectricityPrice;                           // Adjust cost based on electricity produced or consumed

                optimizationResults.Add(new OptimizationResult(
                    unit.Name,
                    condition.TimeFrom,
                    condition.TimeTo,
                    Math.Round(productionAmount, 2),
                    Math.Round(electricityProduced, 2),
                    Math.Round(primaryEnergyConsumption, 2),
                    Math.Round(cost, 2),
                    Math.Round(co2Emissions, 2)
                    ));

                adjustedHeatDemand -= productionAmount;
            }
        }

        if (optimizationResults.Count == 0)
        {
            throw new InvalidOperationException("Optimization failed to produce any results.");
        }

        // // Only returns the results that have been optimized, not the turned off units
        return optimizationResults;
    }
    #endregion

    #region Helper methods
    /// <summary> Simple calculation of heat loss based on distance and building count. </summary>
    private static double CalculateHeatDissipation(int buildings)
    {
        const double HeatLossRate = 0.02;                               // 2% per kilometer
        const double AverageDistance = 300;                             // Average distance in meters
        return buildings * AverageDistance * HeatLossRate / 1000;       // Total heat dissipation
    }

    private static double CalculateShutdownCostFactor(List<ProductionUnit> units, double demand, double electricityPrice)
    {
        const double BaseShutdownCost = 500;                            // Fixed cost for shutting down any unit in DKK
        double totalCapacity = units.Sum(unit => unit.MaxHeat);         // Maximum heating capacity of all units
        double demandCoverageRatio = demand / totalCapacity;            // Determine if demand exceeds total capacity
        double rampDownCost = units.Sum(unit => unit.MaxHeat * 0.05);   // 5% of each unit's max heat as ramp down cost
        double marketAdjustmentCost = demandCoverageRatio > 1 ? (demandCoverageRatio - 1) * electricityPrice * 100 : 0;

        return BaseShutdownCost + rampDownCost + marketAdjustmentCost;  // Total shutdown cost factor
    }

    private static List<ProductionUnit> OrderByCostEffectiveness(List<ProductionUnit> units, double electricityPrice, double shutdownCostFactor) =>
        units.Select(unit => new
        {
            Unit = unit,
            CostEffectiveness = (unit.ProductionCost - (unit.MaxElectricity / unit.MaxHeat * electricityPrice)) * unit.MaxHeat
        })
        .Where(a => a.CostEffectiveness > -shutdownCostFactor)          // Sets the minimum profit potential to cover the shutdown cost
        .OrderBy(a => a.CostEffectiveness)                              // Ordering by ascending cost-effectiveness.
        .Select(a => a.Unit)
        .ToList();
    #endregion
}
