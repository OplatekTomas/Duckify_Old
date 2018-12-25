using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SpotifyAPI.Web.Auth;
using System.Net.Http;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Storage;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpotifyUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page{

        public MainPage(){
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var handle = new EventHandler<SpotifyLogin.SpotifyLoginEventArgs>(async (s, ev) => {
                MessageDialog dialog = new MessageDialog("Player running");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    await RegisterPlayer(ev.TokenInfo.Access_token);
                    await dialog.ShowAsync();
                    MainFrame.Navigate(typeof(EmptyPage));
                });
            });
            MainFrame.Navigate(typeof(SpotifyLogin), handle);
        }

        private async Task RegisterPlayer(string token) {
            var str = File.ReadAllText(Helper.Assets+"\\SpotifyPlayer.html");
            str = str.Replace("ReplaceToken", token);
            Directory.CreateDirectory(Helper.Temp + "\\Web");
            File.WriteAllText(Helper.Temp + "\\Web\\SpotifyPlayer.html", str);
            SpotifyPlayer.Navigate(new Uri("ms-appdata:///temp/Web/SpotifyPlayer.html"));
        }

    }
}
