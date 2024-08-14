using Newtonsoft.Json;

namespace api.Connected_Services
{
    public class CRMService
    {
        private readonly HttpClient _httpClient;

        public CRMService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CrmHttpClient");
        }

        public async Task<string> Get(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return jsonObject["data"].ToString();
        }
    }
}
