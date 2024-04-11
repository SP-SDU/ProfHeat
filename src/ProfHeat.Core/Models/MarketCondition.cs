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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProfHeat.Core.Models;

[XmlRoot("MarketCondition")]
public class MarketCondition()
{
    [XmlElement("TimeFrom")]
    public DateTime TimeFrom { get; set; }          // Start time of the period

    [XmlElement("TimeTo")]
    public DateTime TimeTo { get; set; }            // End time of the period

    [XmlElement("HeatDemand")]
    public double HeatDemand { get; set; }          // The value of the heat demand in MWh

    [XmlElement("ElectricityPrice")]
    public double ElectricityPrice { get; set; }    // The price of electricity per MWh
}
