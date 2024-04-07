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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfHeat.Models;

public class Optimizer
{
    public List<OptimizationResult> Optimize(
        List<HeatingUnit> assets,
        List<HeatDemand> demands,
        List<ElectricityPrice> prices)
    {
        var optimizationResults = new List<OptimizationResult>();

        foreach (var demand in demands)
        {
            var currentPrice = prices.Find(p => p.Time == demand.Time)?.Price ?? 0;

            // Calculate profit potential for each asset based on current electricity price and cost
            var orderedAssets = assets
                .Select(asset => new
                {
                    Asset = asset,
                    ProfitPerMWh = currentPrice - asset.ProductionCost // Assuming electricity price applies to all unit types
                })
                .Where(a => a.ProfitPerMWh > 0) // Only consider assets that can generate profit
                .OrderByDescending(a => a.ProfitPerMWh)
                .Select(a => a.Asset);

            var remainingDemand = demand.DemandValue;

            foreach (var asset in orderedAssets)
            {
                if (remainingDemand <= 0)
                    break;

                var productionAmount = Math.Min(asset.MaxHeat, remainingDemand);
                var cost = productionAmount * asset.ProductionCost;
                var profit = (productionAmount * currentPrice) - cost;

                optimizationResults.Add(new OptimizationResult
                {
                    Time = demand.Time,
                    OptimizedHeat = productionAmount,
                    OptimizedCosts = cost,
                    CO2Emissions = productionAmount * asset.CO2Emission
                });

                remainingDemand -= productionAmount;
            }
        }

        return optimizationResults;
    }
}
