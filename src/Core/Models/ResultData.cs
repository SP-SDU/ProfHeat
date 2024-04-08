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

[XmlRoot("ResultData")]
public class ResultData(DateTime timeFrom, DateTime timeTo, double producedHeat, double consumption, double costs, double co2Emissions)
{
    [XmlElement("TimeFrom")]
    public DateTime TimeFrom { get; set; } = timeFrom;          // Start time of the period

    [XmlElement("TimeTo")]
    public DateTime TimeTo { get; set; } = timeTo;              // End time of the period

    [XmlElement("ProducedHeat")]
    public double ProducedHeat { get; set; } = producedHeat;    // Heat produced in MWh

    [XmlElement("Consumption")]
    public double Consumption { get; set; } = consumption;      // Primary energy consumed in MWh (combines electricity and primary energy consumption)

    [XmlElement("Costs")]
    public double Costs { get; set; } = costs;                  // Production costs, implying net costs considering any profits from electricity generation

    [XmlElement("CO2Emissions")]
    public double CO2Emissions { get; set; } = co2Emissions;    // CO2 emissions in kg
}
