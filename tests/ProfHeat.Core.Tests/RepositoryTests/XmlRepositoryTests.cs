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

public class XmlRepositoryTests : IDisposable
{
    private const string _testFile =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<HeatingGrid xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <Name>Heatington</Name>
    <ImagePath>/Assets/Images/Heatington.svg</ImagePath>
    <Buildings>1600</Buildings>
    <ProductionUnits>
        <ProductionUnit>
            <Name>Gas Boiler</Name>
            <ImagePath>/Assets/Images/GasBoiler.svg</ImagePath>
            <MaxHeat>5</MaxHeat>
            <ProductionCost>500</ProductionCost>
            <CO2Emissions>215</CO2Emissions>
            <GasConsumption>1.01</GasConsumption>
            <MaxElectricity>0</MaxElectricity>
        </ProductionUnit>
        <ProductionUnit>
            <Name>Oil Boiler</Name>
            <ImagePath>/Assets/Images/OilBoiler.svg</ImagePath>
            <MaxHeat>4</MaxHeat>
            <ProductionCost>700</ProductionCost>
            <CO2Emissions>265</CO2Emissions>
            <GasConsumption>1.02</GasConsumption>
            <MaxElectricity>0</MaxElectricity>
        </ProductionUnit>
    </ProductionUnits>
</HeatingGrid>";
    private const string _filePath = "testFile.xml";

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
        var repository = new XmlRepository();
        File.WriteAllText(_filePath, _testFile);

        // Act
        var result = repository.Load<HeatingGrid>(_filePath);

        // Assert
        Assert.Equal("Heatington", result.Name);
        Assert.True(result.ProductionUnits.Count != 0);
        Assert.Equal("Gas Boiler", result.ProductionUnits[0].Name);
    }

    [Fact]
    public void Save_Test()
    {
        // Arrange
        var repository = new XmlRepository();
        var data = new HeatingGrid(
            "Heatington",
            "/Assets/Images/Heatington.svg",
            1600,
            [
                new("Gas Boiler", "/Assets/Images/GasBoiler.svg", 5.00, 500, 215, 1.01, 0),
                new("Oil Boiler", "/Assets/Images/OilBoiler.svg", 4.00, 700, 265, 1.02, 0)
            ]);

        // Act
        repository.Save(data, _filePath);

        // Assert
        Assert.Equal(_testFile.Replace(" ",""), File.ReadAllText(_filePath).Replace(" ", ""));
    }

    [Fact]
    public void Load_ThrowExceptionWhenEmpty()
    {
        // Arrange
        var repository = new XmlRepository();
        File.WriteAllText(_filePath, "");

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => repository.Load<HeatingGrid>(_filePath));
    }
}
