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

using System.Xml.Serialization;

namespace ProfHeat.Core.Models;

[XmlRoot("ProductionUnit")]
public class ProductionUnit(string name, double maxHeat, double productionCost, double co2Emission, double gasConsumption, double maxElectricity)
{
    public Guid Id { get; set; } = Guid.NewGuid();                  // Unique identifier

    [XmlElement("Name")]
    public string Name { get; set; } = name;                        // Name of the heating unit

    [XmlElement("MaxHeat")]
    public double MaxHeat { get; set; } = maxHeat;                  // Maximum heat output in MWh

    [XmlElement("ProductionCost")]
    public double ProductionCost { get; set; } = productionCost;    // Production cost in currency per MWh

    [XmlElement("CO2Emission")]
    public double CO2Emission { get; set; } = co2Emission;          // CO2 emissions in kg per MWh

    [XmlElement("GasConsumption")]
    public double GasConsumption { get; set; } = gasConsumption;    // Gas consumption in units

    [XmlElement("MaxElectricity")]
    public double MaxElectricity { get; set; } = maxElectricity;    // Maximum electricity output in MWh
}