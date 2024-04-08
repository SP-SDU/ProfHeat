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
using System.Xml.Serialization;

namespace ProfHeat.Core.Repositories;

public class AssetManager(string filePath) : IAssetManager
{
    public List<ProductionUnit> LoadAssets()
    {
        var serializer = new XmlSerializer(typeof(List<ProductionUnit>));
        using var reader = new StreamReader(filePath);
        var result = serializer.Deserialize(reader);

        return result as List<ProductionUnit> ?? [];
    }

    public void SaveAssets(List<ProductionUnit> units)
    {
        var serializer = new XmlSerializer(typeof(List<ProductionUnit>));
        using var writer = new StreamWriter(filePath);
        serializer.Serialize(writer, units);
    }
}

