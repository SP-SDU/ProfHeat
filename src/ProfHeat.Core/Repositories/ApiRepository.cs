using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;

using ProfHeat.Core.Interfaces;
using System.Text.Json;

namespace ProfHeat.Core.Repositories;

public class ApiRepository : IRepository
{
    private readonly HttpClient _httpClient = new();

    public T Load<T>(string url)
    {
        var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return JsonSerializer.Deserialize<T>(data)!;
    }

    public void Save<T>(T data, string filePath) => throw new NotImplementedException();
}
