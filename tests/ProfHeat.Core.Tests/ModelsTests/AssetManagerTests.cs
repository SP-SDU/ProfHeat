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
using ProfHeat.Core.Repositories;
using ProfHeat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace ProfHeat.Core.Tests.ModelsTests;
public class AssetManagerTests
{
    [Fact]
    public void Load_ReturnsTest()
    {
        // Arrange
        string expectedFilePath = Path.Combine("Data", "testFile.config");

        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
        XDocument testFile = new XDocument(
            new XElement("HeatingGrid",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                new XElement("Name", "test"),
                new XElement("ImagePath", "/test.svg"),
                new XElement("Buildings", "1000"),
                new XElement("ProductionUnits",
                    new XElement("ProductionUnit",
                        new XElement("Name", "Boiler"),
                        new XElement("ImagePath", "/test2.svg"),
                        new XElement("MaxHeat", "5.00"),
                        new XElement("ProductionCost", "500"),
                        new XElement("CO2Emissions", "215"),
                        new XElement("GasConsumption", "1.01"),
                        new XElement("MaxElectricity", "0")
                    )
                )
            )
        );
        var expected = new HeatingGrid("test", "/test.svg", 1000,
            [new("Boiler", "/test2.svg", 5.00, 500, 215, 1.01, 0)]);

        // Act
        testFile.Save(expectedFilePath);
        IRepository repository = new XmlRepository();
        AssetManager assetManager = new AssetManager(repository, expectedFilePath);
        var loadedData = assetManager.LoadAssets();

        // Assert
        Assert.True(File.Exists(expectedFilePath));
        Assert.Equal(expected.Name, loadedData.Name);

        // Clean up
        File.Delete(expectedFilePath);
    }

    [Fact]
    public void Save_AssetsTest()
    {
        // Arrange
        string expectedFilePath = Path.Combine("Data", "testFile.config");
        var testGrid = new HeatingGrid("test", "/test.svg", 1000,
            [new("Boiler", "/test2.svg", 5.00, 500, 215, 1.01, 0)]);

        // Act
        IRepository repository = new XmlRepository();
        AssetManager assetManager = new AssetManager(repository, expectedFilePath);
        assetManager.SaveAssets(testGrid);

        // Assert
        Assert.True(File.Exists(expectedFilePath));
        Assert.Equal(testGrid.Name, repository.Load<HeatingGrid>(expectedFilePath).Name);

        // Clean up
        File.Delete(expectedFilePath);
    }
}
