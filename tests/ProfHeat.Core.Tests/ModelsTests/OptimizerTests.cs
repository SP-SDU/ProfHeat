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

using ProfHeat.Core.Models;

namespace ProfHeat.Core.Tests.ModelsTests;

public class OptimizerTests
{
    [Fact]
    public void Optimize_ReturnsCorrectResults()
    {
        // Arrange
        var optimizer = new Optimizer();
        var productionUnits = new List<ProductionUnit>
        {
            new("Gas Boiler", "/Assets/Images/GasBoiler.svg", 5.00, 500, 215, 1.01, 0)
        };
        var marketConditions = new List<MarketCondition>
        {
            new(new(2023, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), new(2023, 2, 8, 1, 0, 0, DateTimeKind.Unspecified), 6.62, 1190.94)
        };
        var expectedResult = new OptimizationResult("Gas Boiler", new(2023, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), new(2023, 2, 8, 1, 0, 0, DateTimeKind.Unspecified), 5, 0, 5.05, 2500, 1075);

        // Act
        var result = optimizer.Optimize(productionUnits, marketConditions);

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}
