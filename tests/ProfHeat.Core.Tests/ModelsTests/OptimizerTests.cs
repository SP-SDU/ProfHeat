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

public class Optimizer_Tests
{
    [Fact]
    public void Optimize_ReturnsTest()
    {
        // Arrange
        var grid = new HeatingGrid("TestGrid", "/Path", 1,
            [new("Gas", "/Path", 10.111, 10.111, 1.111, 10.111, 10.111)]);


        var marketConditions = new List<MarketCondition>()
            {
                new(new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified)
                , 10.111, 10.111)
            };

        var optimizer = new Optimizer();
        var expected = new List<OptimizationResult>()
            {
                new ("Gas",new (2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified)
                , 10.11, 10.11, 102.23, 0, 11.23)
            };

        // Act
        var result = optimizer.Optimize(grid, marketConditions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }
}
