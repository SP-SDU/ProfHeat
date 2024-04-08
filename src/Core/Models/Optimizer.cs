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
    public static List<ResultData> Optimize(List<ProductionUnit> units, List<SourceData> sourceData)
    {
        var optimizationResults = new List<ResultData>();

        foreach (var data in sourceData)
        {
            // Calculate profit potential for each unit based on current electricity price and cost
            var orderedUnits = units.Select(unit => new
            {
                Unit = unit,
                ProfitPotential = data.ElectricityPrice - unit.ProductionCost
            })
                .Where(a => a.ProfitPotential > 0)          // Only consider units that can generate profit
                .OrderByDescending(a => a.ProfitPotential)  // Order by profit potential for efficiency
                .Select(a => a.Unit);

            var remainingDemand = data.HeatDemand;

            foreach (var unit in orderedUnits)
            {
                if (remainingDemand <= 0)
                {
                    break;
                }

                var productionAmount = Math.Min(unit.MaxHeat, remainingDemand);
                var cost = productionAmount * unit.ProductionCost;
                var co2Emissions = productionAmount * unit.CO2Emission;
                var consumption = productionAmount * unit.GasConsumption; // Calculate energy consumption based on gas consumption

                optimizationResults.Add(new ResultData(data.TimeFrom, data.TimeTo, productionAmount, consumption, cost, co2Emissions));

                remainingDemand -= productionAmount;
            }
        }

        return optimizationResults;
    }
}
