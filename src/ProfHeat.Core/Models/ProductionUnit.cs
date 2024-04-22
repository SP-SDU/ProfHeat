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
public readonly record struct ProductionUnit(
    [XmlElement("Name")] string Name,                       // Name of the heating unit
    [XmlElement("ImagePath")] string ImagePath,             // Path to the image of the heating unit
    [XmlElement("MaxHeat")] double MaxHeat,                 // Maximum heat output in MWh
    [XmlElement("ProductionCost")] double ProductionCost,   // Production cost in currency per MWh
    [XmlElement("CO2Emissions")] double CO2Emissions,       // CO2 emissions in kg per MWh
    [XmlElement("GasConsumption")] double GasConsumption,   // Gas consumption in units
    [XmlElement("MaxElectricity")] double MaxElectricity    // Maximum electricity output in MWh
    );
