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

namespace ProfHeat.Core.Tests.RepositoryTests;

public class CsvRepositoryTests : IDisposable
{
    private const string _testFile =
        "UnitName,TimeFrom,TimeTo,ProducedHeat,ElectricityProduced,GasConsumption,Costs,CO2Emissions\r\n" +
        "Gas Boiler,02/08/2023 00:00:00,02/08/2023 01:00:00,5,0,5.05,2500,1075\r\n";
    private const string _filePath = "testFile.csv";

    public CsvRepositoryTests() => File.WriteAllText(_filePath, _testFile);

    protected virtual void Dispose(bool disposing) => File.Delete(_filePath);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Load_Test()
    {
        // Arrange
        var repository = new CsvRepository();

        // Act
        var result = repository.Load<List<OptimizationResult>>(_filePath);

        // Assert
        var expected = result[0];
        Assert.Equal("Gas Boiler", expected.UnitName);
        Assert.Equal(new DateTime(2023, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), expected.TimeFrom);
        Assert.Equal(new DateTime(2023, 2, 8, 1, 0, 0, DateTimeKind.Unspecified), expected.TimeTo);
        Assert.Equal(5, expected.ProducedHeat);
        Assert.Equal(0, expected.ElectricityProduced);
        Assert.Equal(5.05, expected.GasConsumption);
        Assert.Equal(2500, expected.Costs);
        Assert.Equal(1075, expected.CO2Emissions);
    }

    [Fact]
    public void Save_Test()
    {
        // Arrange
        var repository = new CsvRepository();
        var data = new List<OptimizationResult>
        {
            new("Gas Boiler", new DateTime(2023, 2, 8, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2023, 2, 8, 1, 0, 0, DateTimeKind.Unspecified), 5, 0, 5.05, 2500, 1075)
        };

        // Act
        repository.Save(data, _filePath);

        // Assert
        var fileContent = File.ReadAllText(_filePath);
        Assert.Equal(_testFile, fileContent);
    }
}
