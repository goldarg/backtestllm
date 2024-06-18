using Newtonsoft.Json;

namespace api.DataAccess;

public class AccessTokenHandler : DelegatingHandler
{
    private static readonly object OLock = new();
    private static AccessTokenHandler? _instance;
    private DateTime? _tokenExpiry;
    public string? AccessToken;

    public static AccessTokenHandler Instance
    {
        get
        {
            lock (OLock)
            {
                _instance ??= new AccessTokenHandler();

                if (_instance._tokenExpiry == null || _instance._tokenExpiry <= DateTime.Now) UpdateAccessToken();

                return _instance;
            }
        }
    }

    private static void UpdateAccessToken()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://accounts.zoho.com/");

        //Esto luego habria que pasarlo a setting
        var postData = new Dictionary<string, string>
        {
            { "client_id", "1000.YM5NSNFPY1T90Q6Y4UQA0IGKIZRRYJ" },
            { "client_secret", "7775ca8868f70e299a4fcd10398835adb2423c5f45" },
            { "refresh_token", "1000.cd7bf5896bec77abe5e1932fc79f0d9e.826a80631a722bd58f633e1fb8cde31a" },
            { "grant_type", "refresh_token" }
        };

        var addMe = new FormUrlEncodedContent(postData);

        var response = client.PostAsync("oauth/v2/token", addMe).Result;
        var json = response.Content.ReadAsStringAsync().Result;

        dynamic result = JsonConvert.DeserializeObject(json) ??
                         throw new InvalidOperationException("fallo al deserealizar token");
        var accessToken = result["access_token"].Value;

        if (_instance == null) return;
        _instance.AccessToken = accessToken;
        _instance._tokenExpiry = DateTime.Now.AddMinutes(50); //Margen de 10 minutos
    }
}