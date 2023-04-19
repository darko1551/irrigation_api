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
        const string identityUrl = "https://id.mobilisis.com";
        const string clientId = "darko-debeljak-api";
        const string clientSecret = "cG3Y09xILbOsJ4atzdF2mP7a5Ins66iM";
        const string credentialsScope = "openid profile email roles web-origins";
        const string tokenEndpoint = "https://id.mobilisis.com/auth/realms/mobilisis.global/protocol/openid-connect/token";


        const string username = "ddebeljak@mobilisis.hr";
        const string password = "Darko123";

        private readonly HttpClient _httpClient;

        

        public ValveClient( HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://ingress.mobilisis.com/platform/management/v1.0/");
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


        /*private async Task RefreshAccessToken()
        {

            _httpClient.BaseAddress = new Uri(identityUrl);
            TokenResponse tokenResponse = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                GrantType = "refresh_token", 
                ClientId = clientId, 
                ClientSecret = clientSecret, 
                RefreshToken = ClientCredentials.token!.RefreshToken
            });
            ClientCredentials.token = tokenResponse;
        }*/

        /*public async Task getOrRefreshToken() {
            if (ClientCredentials.token == null)
            {
                await getAccessToken();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientCredentials.token!.AccessToken}");
            }
            else if (ClientCredentials.token.ExpiresIn < 30) {
                await RefreshAccessToken();
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ClientCredentials.token!.AccessToken}");

            }

        }*/

        public bool openValve(string guid) {
            /* await getOrRefreshToken();
             if (ClientCredentials.token!.IsError) {
                 return false;
             }*/

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
            /*await getOrRefreshToken();
            if (ClientCredentials.token!.IsError) {
                return false;
            }*/

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
            /*await getOrRefreshToken();
            if (ClientCredentials.token!.IsError) {
                return false;
            }*/

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
