using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Duckify {
    public partial class Spotify {
        public class Auth {

            public static async Task<TokenInfo> RefreshToken(TokenInfo token) {
                var refreshed_token = await SendAuthRequest("grant_type=refresh_token&refresh_token=" + token.Refresh_token);
                if(refreshed_token == null) {
                    return null;
                }
                refreshed_token.Refresh_token = token.Refresh_token;
                return refreshed_token;
            }

            public static async Task<TokenInfo> GetToken(string accessCode) {
                return await SendAuthRequest("grant_type=authorization_code&code=" + accessCode + "&redirect_uri=http://127.0.0.1:8000");
            }

            private static async Task<TokenInfo> SendAuthRequest(string body) {
                using (HttpClient client = new HttpClient()) {
                    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                    //Build url encoded body
                    var content = new StringContent(body);
                    message.Content = content;
                    //Refresh header to correct value, yes this probably could be simplified.
                    content.Headers.Remove("Content-Type");
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    //Add Base64 encoded app info
                    message.Headers.Add("Authorization", "Basic " + Helper.EncodeTo64("146234f4fccf47ffbe4de27b8b472ce8:f8b159ba1f324d90bc4f7caa74fb2a05"));
                    var response = await client.SendAsync(message);
                    if (response.IsSuccessStatusCode) {
                        //Get response string and parse it
                        string res = await response.Content.ReadAsStringAsync();
                        return JObject.Parse(res).ToObject<TokenInfo>();
                    } else {
                        return null;
                    }
                }
            }

        }
    }
}
