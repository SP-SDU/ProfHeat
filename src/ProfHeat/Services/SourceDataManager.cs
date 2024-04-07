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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ProfHeat.Interfaces;
using ProfHeat.Models;

namespace ProfHeat.Services;

public class SourceDataManager(string filePath) : ISourceDataManager
{
    private readonly char _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

    // Loads data from a CSV file.
    public List<(HeatDemand, ElectricityPrice)> LoadSourceData()
    {
        var data = new List<(HeatDemand, ElectricityPrice)>();
        foreach (var line in File.ReadAllLines(filePath).Skip(1).ToList()) // Skips the headers.
        {
            var columns = line.Split(_delimiter);

            // Parse Heat Demand and Electricity Price from CSV
            var heatDemand = new HeatDemand(
                DateTime.Parse(columns[0]),
                double.Parse(columns[2], CultureInfo.InvariantCulture));

            var electricityPrice = new ElectricityPrice(
                DateTime.Parse(columns[0]),
                double.Parse(columns[3], CultureInfo.InvariantCulture));

            data.Add((heatDemand, electricityPrice));
        }

        return data;
    }

    // Saves to a CSV.
    public void SaveSourceData(List<(HeatDemand demand, ElectricityPrice price)> data)
    {
        var csv = new StringBuilder();
        _ = csv.AppendLine($"Time from (DKK local time){_delimiter}Time to (DKK local time){_delimiter}Heat Demand (MWh){_delimiter}Electricity Price (DKK / Mwh el)");

        foreach (var (demand, price) in data)
        {
            _ = csv.AppendLine($"{demand.Time}{_delimiter}{demand.Time.AddHours(1)}{_delimiter}{demand.DemandValue}{_delimiter}{price.Price}");
        }

        File.WriteAllText(filePath, csv.ToString());
    }
}
