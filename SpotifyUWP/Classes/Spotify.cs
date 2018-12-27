using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpotifyUWP {
    public partial class Spotify {

        #region Variables
        private static TokenInfo _accessToken;

        public static TokenInfo AccessToken {
            get {
                if (_accessToken.ValidUntil < DateTime.Now) {
                    _accessToken = Auth.RefreshToken(_accessToken).GetAwaiter().GetResult();
                }
                return _accessToken;
            }
            set {
                value.ValidUntil = DateTime.Now.AddSeconds(value.Expires_in);
                _accessToken = value;
            }
        }

        public static SpotifyWebAPI Client { get; set; }

        private static Timer refreshTokenTimer;

        private static Timer _refreshCurrentSongTimer = new Timer(200);

        public static string  DeviceId { get; set; }

        #endregion

        public static void InitClient(TokenInfo token) {
            AccessToken = token;
            if(!string.IsNullOrEmpty(AccessToken.Access_token)) {
                Client = new SpotifyWebAPI {
                    AccessToken = AccessToken.Access_token,
                    TokenType = AccessToken.Token_type
                };
                //Create timer that will fire minute before the token would become invalid;
                refreshTokenTimer = new Timer((AccessToken.ValidUntil.AddMinutes(-1) - DateTime.Now).TotalMilliseconds);
                refreshTokenTimer.Elapsed += async (s, ev) => await RefreshTokenTimerElapsed();
                refreshTokenTimer.Start();
                //Run the current song udpate timer
                _refreshCurrentSongTimer.Elapsed += (s, ev) => UpdateData();
                _refreshCurrentSongTimer.Start();
                Queue.CurrentSong = Client.GetPlayingTrack();
            } else {
                throw new Exception("Access Token was not recieved yet");
            }
        }

        private async static Task RefreshTokenTimerElapsed() {
            //Stop current running timer and dispose of it
            refreshTokenTimer.Stop();
            refreshTokenTimer.Dispose();
            //Refresh the damn token
            AccessToken = await Auth.RefreshToken(_accessToken);
            //Refresh Client with new Token data
            Client.AccessToken = AccessToken.Access_token;
            Client.TokenType = AccessToken.Token_type;
            //Create the timer again
            refreshTokenTimer = new Timer((AccessToken.ValidUntil.AddMinutes(-1) - DateTime.Now).TotalMilliseconds);
            refreshTokenTimer.Elapsed += async (s, ev) => await RefreshTokenTimerElapsed();
            refreshTokenTimer.Start();
        }


        private static int updateCount = 0;

        private static async Task UpdateData() {
            await Task.Run(() => {
                updateCount++;
                //This method runs every 200ms, 200*15 = 3000?;
                if (updateCount % 15 == 0) {
                    Queue.CurrentSong = Client.GetPlayingTrack();
                }
                if(Queue.CurrentSong != null && Queue.CurrentSong.Item != null) {
                    if (Queue.CurrentSong.IsPlaying) {
                        Queue.CurrentSong.ProgressMs += 100;
                        if ((Queue.CurrentSong.Item.DurationMs - Queue.CurrentSong.ProgressMs) < 1000) {
                            Queue.SongChanged?.Invoke(Queue.CurrentSong.Item.Name, new EventArgs());
                        }
                    }
                }             
            });
        }
    }
}
