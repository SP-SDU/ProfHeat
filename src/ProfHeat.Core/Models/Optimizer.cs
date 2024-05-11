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
    /// <summary> Greedy Algorithm to optimize heat production, minimizing costs and meeting demand. </summary>
    public List<OptimizationResult> Optimize(List<ProductionUnit> productionUnits, List<MarketCondition> marketConditions)
    {
        var optimizationResults = new List<OptimizationResult>();

        foreach (var condition in marketConditions)
        {
            var remainingDemand = condition.HeatDemand;

            foreach (var unit in OrderByNetCost(productionUnits, condition.ElectricityPrice))
            {
                var productionAmount = (remainingDemand > 0) ? Math.Min(unit.MaxHeat, remainingDemand) : 0; // Only calculate production if there is demand
                var electricityProduced = (productionAmount / unit.MaxHeat) * unit.MaxElectricity;
                var costs = (productionAmount * unit.ProductionCost) - (electricityProduced * condition.ElectricityPrice);
                var co2Emissions = productionAmount * unit.CO2Emissions;
                var GasConsumption = productionAmount * unit.GasConsumption;

                optimizationResults.Add(new OptimizationResult(
                    unit.Name,
                    condition.TimeFrom,
                    condition.TimeTo,
                    Math.Round(productionAmount, 2),
                    Math.Round(electricityProduced, 2),
                    Math.Round(GasConsumption, 2),
                    Math.Round(costs, 2),
                    Math.Round(co2Emissions, 2)
                ));

                remainingDemand -= productionAmount;
            }
        }

        if (optimizationResults.Count == 0)
        {
            throw new InvalidOperationException("Optimization failed to produce any results.");
        }

        return optimizationResults;
    }
    /// <summary> Orders production units by net cost, adjusted for electricity price. </summary>
    private static List<ProductionUnit> OrderByNetCost(List<ProductionUnit> units, double electricityPrice) =>
        [.. units.OrderBy(unit => unit.ProductionCost - (unit.MaxElectricity / unit.MaxHeat * electricityPrice))];
}
