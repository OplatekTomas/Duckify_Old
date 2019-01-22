using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using HttpListener = System.Net.Http.HttpListener;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Web;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Duckify {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpotifyLogin : Page {
        public SpotifyLogin() {
            this.InitializeComponent();
        }

        public event EventHandler<SpotifyLoginEventArgs> Finished;

        public class SpotifyLoginEventArgs : EventArgs {
            public Spotify.TokenInfo TokenInfo { get; set; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Finished += (EventHandler<SpotifyLoginEventArgs>)e.Parameter;
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e) {
            HttpListener listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 8000);
            listener.Request += async (s, ev) => await Callback(ev);
            Finished += (s, ev) => {
                listener.Close();
            };
            listener.Start();
            Browser.Navigate(new Uri("http://accounts.spotify.com/authorize?client_id=146234f4fccf47ffbe4de27b8b472ce8&response_type=code&redirect_uri=http://127.0.0.1:8000&scope=streaming%20user-read-birthdate%20user-read-email%20user-read-private%20user-read-playback-state"));
        }

        private async Task Callback(HttpListenerRequestEventArgs args) {
            args.Response.Close();
            if (args.Request.Url.Query.StartsWith("?code=")) {
                string authCode = args.Request.Url.Query.Substring(6);
                var token = await Spotify.Auth.GetToken(authCode);
                Spotify.AccessToken = token;
                Finished.Invoke(this, new SpotifyLoginEventArgs() { TokenInfo = token });
                
            }
        }
    }
}
