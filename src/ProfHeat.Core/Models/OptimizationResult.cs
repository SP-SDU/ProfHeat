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

using CsvHelper.Configuration.Attributes;

namespace ProfHeat.Core.Models;

public readonly record struct OptimizationResult(
    [Name("TimeFrom")] DateTime TimeFrom,                               // Start time of the period
    [Name("TimeTo")] DateTime TimeTo,                                   // End time of the period
    [Name("ProducedHeat")] double ProducedHeat,                         // Heat produced in MWh
    [Name("ElectricityProduced")] double ElectricityProduced,           // Added for clarity on electricity produced or consumed
    [Name("PrimaryEnergyConsumption")] double PrimaryEnergyConsumption, // Primary energy used
    [Name("Costs")] double Costs,                                       // Production costs, implying net costs considering any profits from electricity generation
    [Name("CO2Emissions")] double CO2Emissions                          // CO2 emissions in kg
    );
