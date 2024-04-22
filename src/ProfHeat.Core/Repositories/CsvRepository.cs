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

using CsvHelper;
using ProfHeat.Core.Interfaces;

namespace ProfHeat.Core.Repositories;

// Works for both single objects and collections but not for nested collections due to Csv not supporting it
// Uses international/American delimiter settings
public class CsvRepository : IRepository
{
    public T Load<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        if (IsCollection(typeof(T)))
        {
            // Extract the type of the elements in the list
            Type itemType = typeof(T).GetGenericArguments().Single();
            var records = csv.GetRecords(itemType).ToList();

            // Create a list of the appropriate type and return it
            var listType = typeof(List<>).MakeGenericType(itemType);
            var list = Activator.CreateInstance(listType);

            foreach (var record in records)
            {
                _ = ((IList) list!).Add(record);
            }

            return (T) list!;
        }
        else
        {
            var record = csv.GetRecord<T>();
            return record ?? throw new InvalidOperationException("Deserialization returned null or file is empty.");
        }
    }

    public void Save<T>(T data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (data is not IEnumerable)
        {
            csv.WriteRecord(data);
            csv.NextRecord();
        }
        else
        {
            csv.WriteRecords((IEnumerable) data);
        }
    }

    private static bool IsCollection(Type type) => type.IsGenericType && type.GetInterfaces().ToList()
        .Exists(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
}
