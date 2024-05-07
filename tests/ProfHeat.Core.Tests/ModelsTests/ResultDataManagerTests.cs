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
public class ResultDataManagerTests
{
    [Fact]
    public void LoadResultData_Test()
    {
        // Arrange
        string testPath = Path.Combine("Data", "testFileRDM_Load.csv");
        var expectedData = new List<OptimizationResult>()
            {
                new ("Gas",new (2023, 02, 08, 0, 0, 0),
                new DateTime(2023, 02, 08, 1, 0, 0), 10.11, 10.11, 102.23, 0, 11.23)
            };
        var repository = new CsvRepository();
        var _result = new ResultDataManager(repository);

        // Act
        repository.Save(expectedData, testPath);
        var result = _result.LoadResultData(testPath);

        // Assert
        Assert.Equal(expectedData, result);

        // Clean up
        File.Delete(testPath);
    }

    [Fact]
    public void SaveResultData_Test()
    {
        // Arrange
        string testPath = Path.Combine("Data", "testFileRDM_Save.csv");
        var expectedData = new List<OptimizationResult>()
            {
                new ("Gas",new (2023, 02, 08, 0, 0, 0),
                new DateTime(2023, 02, 08, 1, 0, 0), 10.11, 10.11, 102.23, 0, 11.23)
            };
        var repository = new CsvRepository();
        var _result = new ResultDataManager(repository);

        // Act
        _result.SaveResultData(expectedData, testPath);
        var result = repository.Load<List<OptimizationResult>>(testPath);

        // Assert
        Assert.Equal(expectedData, result);

        // Clean up
        File.Delete(testPath);
    }
}
