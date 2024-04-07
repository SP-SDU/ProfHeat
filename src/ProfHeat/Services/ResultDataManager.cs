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
using ProfHeat.Models;
using ProfHeat.Interfaces;

namespace ProfHeat.Services;

public class ResultDataManager(string filePath) : IResultDataManager
{
    private readonly char _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

    // Loads optimization results from a CSV file.
    public List<OptimizationResult> LoadOptimizationResults()
    {
        var results = new List<OptimizationResult>();
        foreach (var line in File.ReadAllLines(filePath).Skip(1).ToList()) // Skip the header line
        {
            var columns = line.Split(_delimiter);

            var result = new OptimizationResult(
                DateTime.Parse(columns[0].Trim()),
                double.Parse(columns[1].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[2].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[3].Trim(), CultureInfo.InvariantCulture));

            results.Add(result);
        }

        return results;
    }

    // Saves optimization results to a CSV file.
    public void SaveOptimizationResults(List<OptimizationResult> results)
    {
        var sb = new StringBuilder();
        _ = sb.AppendLine($"Time{_delimiter}OptimizedHeat{_delimiter}OptimizedCosts{_delimiter}CO2Emissions");

        foreach (var result in results)
        {
            _ = sb.AppendLine($"{result.Time}{_delimiter}{result.OptimizedHeat}{_delimiter}{result.OptimizedCosts}{_delimiter}{result.CO2Emissions}");
        }

        File.WriteAllText(filePath, sb.ToString());
    }
}

