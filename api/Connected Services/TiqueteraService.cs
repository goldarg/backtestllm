using System.Text.Json;

namespace api.Connected_Services
{
    public class TiqueteraService
    {
        private readonly HttpClient _httpClient;

        public TiqueteraService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TiqueteraHttpClient");
        }

        public async Task<string> Get(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };

            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
            return jsonObject["data"].ToString();
        }
    }
}
