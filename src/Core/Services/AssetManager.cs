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

using ProfHeat.Core.Models;
using ProfHeat.Core.Interfaces;
using System.Globalization;
using System.Text;

namespace ProfHeat.Core.Services;

public class AssetManager(string filePath) : IAssetManager
{
    private readonly char _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

    public List<ProductionUnit> LoadAssets()
    {
        var units = new List<ProductionUnit>();
        foreach (var line in File.ReadAllLines(filePath).Skip(1).ToList()) // Skip the header line
        {
            var columns = line.Split(_delimiter);
            var unit = new ProductionUnit(
                columns[0].Trim(),
                double.Parse(columns[1].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[2].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[3].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[4].Trim(), CultureInfo.InvariantCulture),
                double.Parse(columns[5].Trim(), CultureInfo.InvariantCulture));

            units.Add(unit);
        }

        return units;
    }

    public void SaveAssets(List<ProductionUnit> units)
    {
        var sb = new StringBuilder("Name,Type,MaxHeat,ProductionCost,CO2Emission,GasConsumption,MaxElectricity\n");
        foreach (var unit in units)
        {
            _ = sb.AppendLine($"{unit.Name}{_delimiter}{unit.MaxHeat}{_delimiter}{unit.ProductionCost}{_delimiter}{unit.CO2Emission}{_delimiter}{unit.GasConsumption}{_delimiter}{unit.MaxElectricity}");
        }

        File.WriteAllText(filePath, sb.ToString());
    }
}

