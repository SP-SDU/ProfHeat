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
using ProfHeat.DAL.Interfaces;

namespace ProfHeat.DAL.Repositories;

public class XmlRepository : IXmlRepository
{
    public T Load<T>(string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(filePath);
            var result = serializer.Deserialize(reader) ?? throw new InvalidOperationException("Deserialization returned null.");
            return (T) result;
        }
        catch (FileNotFoundException fnfe)
        {
            throw new ApplicationException($"The file '{filePath}' was not found.", fnfe);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error loading data from {filePath}.", ex);
        }
    }

    public void Save<T>(T data, string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, data);
        }
        catch (UnauthorizedAccessException uae)
        {
            throw new ApplicationException($"Access to '{filePath}' is denied.", uae);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error saving data to {filePath}.", ex);
        }
    }
}
