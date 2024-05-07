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
using System.Text;

namespace ProfHeat.Core.Tests.ModelsTests;
public class CsvRepositoryTests
{
    [Fact]
    public void Load_ReturnsData()
    {
        // Arrange
        string expectedFilePath = Path.Combine("Data", "testFileCSV_Load.csv");
        var testData = new List<MarketCondition>
        {
            new (new DateTime(2023, 8, 2, 0, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2023, 8, 2, 1, 0, 0, DateTimeKind.Unspecified)
            , 10.111, 10.111)
        };
        string header = "TimeFrom,TimeTo,HeatDemand,ElectricityPrice";
        StringBuilder test = new StringBuilder();
        test.AppendLine(header);
        foreach (var item in testData)
        {
            test.AppendLine($"{item.TimeFrom},{item.TimeTo},{item.HeatDemand},{item.ElectricityPrice}");
        }
        File.WriteAllText(expectedFilePath, test.ToString());
        CsvRepository repository = new CsvRepository();

        // Act
        var result = repository.Load<List<MarketCondition>>(expectedFilePath);

        // Assert
        Assert.Equal(testData.Count, result.Count);
        for (int i = 0; i < testData.Count; i++)
        {
            Assert.Equal(testData[i].TimeFrom, result[i].TimeFrom);
            Assert.Equal(testData[i].ElectricityPrice, result[i].ElectricityPrice);
        }

        // Clean up
        File.Delete(expectedFilePath);
    }

    [Fact]
    public void Save_CreatesFile()
    {
        // Arrange
        string expectedFilePath = Path.Combine("Data", "testFileCSV_Save.csv");
        var testData = new List<MarketCondition>
        {
            new (new DateTime(2023, 8, 2, 0, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2023, 8, 2, 1, 0, 0, DateTimeKind.Unspecified)
            , 10.111, 10.111)
        };
        CsvRepository repository = new CsvRepository();

        // Act
        repository.Save(testData, expectedFilePath);
        var expected = File.ReadAllLines(expectedFilePath).ToList().Skip(1).First().Split(',');

        // Assert
        Assert.True(File.Exists(expectedFilePath));
        Assert.Equal(testData[0].TimeFrom, DateTime.Parse(expected[0]));

        // Clean up
        File.Delete(expectedFilePath);
    }
}
