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

namespace ProfHeat.Models;

public enum HeatingUnitType
{
    Gas,
    Oil,
    Electric
}

public class HeatingUnit
{
    public Guid Id { get; set; } // Unique identifier
    public required string Name { get; set; } // Name of the heating unit
    public HeatingUnitType Type { get; set; } // Type of the heating unit
    public double MaxHeat { get; set; } // Maximum heat output in MWh
    public double ProductionCost { get; set; } // Production cost in currency per MWh
    public double MaxElectricity { get; set; } // Maximum electricity output in MWh
    public double CO2Emission { get; set; } // CO2 emissions in kg per MWh
    public double GasConsumption { get; set; } // Gas consumption in units
}
