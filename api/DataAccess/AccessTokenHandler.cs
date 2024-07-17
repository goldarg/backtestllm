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
            { "client_id", "1000.T0JQBZBHU16DOVJ76TQG4MGTLZQF2H" },
            { "client_secret", "bc061baaf6ca19fdb603e770c0213103cb06995d1c" },
            { "refresh_token", "1000.0cc69ee9667401b756c2b2a228a3c3dc.3217f8a5e75a972ee62fe54993e41705" },
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