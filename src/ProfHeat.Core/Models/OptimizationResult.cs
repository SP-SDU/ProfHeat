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

[XmlRoot("OptimizationResult")]
public class OptimizationResult()
{
    [XmlElement("TimeFrom")]
    public DateTime TimeFrom { get; set; }                  // Start time of the period

    [XmlElement("TimeTo")]
    public DateTime TimeTo { get; set; }                    // End time of the period

    [XmlElement("ProducedHeat")]
    public double ProducedHeat { get; set; }                // Heat produced in MWh

    [XmlElement("ElectricityProduced")]
    public double ElectricityProduced { get; set; }         // Added for clarity on electricity produced or consumed

    [XmlElement("PrimaryEnergyConsumption")]
    public double PrimaryEnergyConsumption { get; set; }    // Primary energy used

    [XmlElement("Costs")]
    public double Costs { get; set; }                       // Production costs, implying net costs considering any profits from electricity generation

    [XmlElement("CO2Emissions")]
    public double CO2Emissions { get; set; }                // CO2 emissions in kg
}
