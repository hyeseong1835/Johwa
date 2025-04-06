using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Johwa.Core;

namespace Johwa.Rest;

public class DiscordRestClient
{
    const string baseUrl = "https://discord.com/api/v10/";

    public readonly DiscordBot bot;

    readonly HttpClient client;
    readonly JsonSerializerOptions jsonOptions = 
        new JsonSerializerOptions() { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

    public DiscordRestClient(DiscordBot bot)
    {
        this.bot = bot;
        client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot.token);
    }

    #region Raw API

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
    public async Task<T?> PostAsync<T>(string endpoint, object payload)
    {
        StringContent content = new StringContent(
            JsonSerializer.Serialize(payload, jsonOptions), 
            Encoding.UTF8, 
            "application/json"
        );
        HttpResponseMessage response = await client.PostAsync(baseUrl + endpoint, content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
    public async Task PatchAsync(string endpoint, object payload)
    {
        StringContent content = new StringContent(
            JsonSerializer.Serialize(payload, jsonOptions), 
            Encoding.UTF8, 
            "application/json"
        );
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), baseUrl + endpoint) { Content = content };
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    public async Task DeleteAsync(string endpoint)
    {
        HttpResponseMessage response = await client.DeleteAsync(baseUrl + endpoint);
        response.EnsureSuccessStatusCode();
    }
    
    #endregion
}