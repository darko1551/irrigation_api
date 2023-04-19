using IdentityModel.Client;
using irregation_api.Static;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace irregation_api
{
    public class ValveClient
    {
        private readonly IConfiguration _configuration;

        string identityUrl;
        string clientId;
        string clientSecret;
        string credentialsScope;
        string tokenEndpoint;
        string baseAdress;
        string username;
        string password;

        private readonly HttpClient _httpClient;

        

        public ValveClient( HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            identityUrl = _configuration.GetValue<string>("Authentication:identityUrl");
            clientId = _configuration.GetValue<string>("Authentication:clientId");
            clientSecret = _configuration.GetValue<string>("Authentication:clientSecret");
            credentialsScope = _configuration.GetValue<string>("Authentication:credentialsScope");
            tokenEndpoint = _configuration.GetValue<string>("Authentication:tokenEndpoint");
            baseAdress = _configuration.GetValue<string>("Authentication:baseAdress");
            username = _configuration.GetValue<string>("Authentication:username");
            password = _configuration.GetValue<string>("Authentication:password");
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseAdress);
        }


        public async Task<TokenResponse> getAccessToken() {

            if (_httpClient.BaseAddress == null) {
                _httpClient.BaseAddress = new Uri(identityUrl);
            }
            TokenResponse tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest {
            UserName = username,
            Password = password,
            GrantType = "password",
            ClientId = clientId,
            ClientSecret = clientSecret,
            Address = tokenEndpoint
            });
            ClientCredentials.token = tokenResponse;
            return tokenResponse;
        }

        public bool openValve(string guid) {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientCredentials.token!.AccessToken}");

            DateTime expiration = DateTime.Now.AddSeconds(70);

            var values = new Dictionary<string, dynamic> {
                { "deviceUuid", guid },
                { "clientId", null},
                { "requestTimeout",0},
                { "data", new Dictionary<string, string> { { "base64", "YWIub3BlbnY=" }, { "expireTime", expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") } } }
            };
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tok}");

            HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var response = _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}api/device-actions/commit", values).Result;
            if (response.IsSuccessStatusCode) {
                return true;
            }
            return false;
        }


        public bool closeValve(string guid)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientCredentials.token!.AccessToken}");

            
            DateTime expiration = DateTime.Now.AddSeconds(70);

            var values = new Dictionary<string, dynamic> {
                { "deviceUuid", guid },
                { "clientId", null},
                { "requestTimeout",0},
                { "data", new Dictionary<string, string> { { "base64", "YWIuY2xvc2V2" }, { "expireTime", expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") } } }
            };
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tok}");

            HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}api/device-actions/commit", values).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }



        public async Task<String?> getUuid(string mac)
        {

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientCredentials.token!.AccessToken}");


            String uuid;
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/api/devices/mac/{mac}");
            string responseString = await response.Content.ReadAsStringAsync();
            try
            {
                dynamic data = JObject.Parse(responseString);
            
                uuid = data.uuid; 
            }
            catch (Exception ex) {
                return null;
            }

            if (response.IsSuccessStatusCode && uuid != "")
            {
                return uuid;
            }
            return null;
        }
    }
}
