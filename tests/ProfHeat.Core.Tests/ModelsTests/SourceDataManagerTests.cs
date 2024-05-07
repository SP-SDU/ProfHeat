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
using ProfHeat.Core.Repositories;

namespace ProfHeat.Core.Tests.ModelsTests;
public class SourceDataManagerTests
{
    [Fact]
    public void LoadSourceData_Test()
    {
        // Arrange
        string testFilePath = Path.Combine("Data", "testFileSDM_Load.csv");
        var marketConditions = new List<MarketCondition>()
            {
                new(new DateTime(2023, 02, 08, 0, 0, 0),
                new DateTime(2023, 02, 08, 1, 0, 0), 10.111, 10.111)
            };
        var repository = new CsvRepository();
        var sourceDataManager = new SourceDataManager(repository);

        // Act
        repository.Save(marketConditions, testFilePath);
        var expectedData = marketConditions.ToList();
        var result = sourceDataManager.LoadSourceData(testFilePath);

        // Assert
        Assert.Equal(expectedData, result);

        // Clean up
        File.Delete(testFilePath);
    }

    [Fact]
    public void SaveSourceData_Test()
    {
        // Arrange
        string testFilePath = Path.Combine("Data", "testFileSDM_Save.csv");
        var marketConditions = new List<MarketCondition>()
            {
                new(new DateTime(2023, 02, 08, 0, 0, 0),
                new DateTime(2023, 02, 08, 1, 0, 0), 10.111, 10.111)
            };
        var repository = new CsvRepository();
        var sourceDataManager = new SourceDataManager(repository);

        // Act
        sourceDataManager.SaveSourceData(marketConditions, testFilePath);
        var expectedData = marketConditions.ToList();
        var result = repository.Load<List<MarketCondition>>(testFilePath);

        // Assert
        Assert.Equal(expectedData, result);

        // Clean up
        File.Delete(testFilePath);
    }
}
