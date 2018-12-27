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
using SpotifyAPI.Web;
using Windows.ApplicationModel.Activation;
using System.Reflection;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
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


        private void Page_Loaded(object sender, RoutedEventArgs e) {
            var handle = new EventHandler<SpotifyLogin.SpotifyLoginEventArgs>(async (s, ev) => {
                Spotify.InitClient(ev.TokenInfo);
                await RegisterPlayer(ev.TokenInfo.Access_token);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    await LoadUI();
                });
                Server.Init();

            });
            Navigation.IsEnabled = false;
            ContentFrame.Navigate(typeof(SpotifyLogin), handle);

        }

        private async Task LoadUI() {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                ContentFrame.Navigate(typeof(QueuePlayer));
                ContentFrame.BackStack.Clear();
                Navigation.IsEnabled = true;
                await RefreshView();
                Queue.SongChanged += async (s1, ev1) => {
                    await Task.Delay(3000);
                    await RefreshView();
                };
            });
        }


        private async Task RefreshView() {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if(Queue.CurrentSong.Item != null) {
                    SongName.Text = Queue.CurrentSong.Item.Name;
                    ArtistNames.Text = Helper.BuildListString(Queue.CurrentSong.Item.Artists.Select(x => x.Name));
                    CoverArt.Source = new BitmapImage(new Uri(Queue.CurrentSong.Item.Album.Images[0].Url));
                }

            });

        }


        private async Task RegisterPlayer(string token) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                var str = File.ReadAllText(Helper.Assets + "\\SpotifyPlayer.html");
                str = str.Replace("ReplaceToken", token);
                Directory.CreateDirectory(Helper.Temp + "\\Web");
                File.WriteAllText(Helper.Temp + "\\Web\\SpotifyPlayer.html", str);
                SpotifyClientBackground.Navigate(new Uri("ms-appdata:///temp/Web/SpotifyPlayer.html"));
            });
            //Local function that switches spotify to play in this application
            async Task function() {
                Device device = null;
                while (device == null) {
                    await Task.Delay(500);
                    AvailabeDevices devices = await Spotify.Client.GetDevicesAsync();
                    device = devices.Devices.FirstOrDefault(x => x.Name == "Duckify");
                }
                await Spotify.Client.TransferPlaybackAsync(device.Id);
                Spotify.DeviceId = device.Id;
            }
            Task.Run(function);


        }


        private Dictionary<string, Type> _pages = null;
        
        private Dictionary<string, Type> GeneratePages() {
            string name = Assembly.GetExecutingAssembly().GetName().Name;
            Dictionary<string, Type> pages = new Dictionary<string, Type>();
            foreach(NavigationViewItem item in Navigation.MenuItems) {
                pages.Add((string)item.Tag, Type.GetType(name + "." + item.Tag));
            }
            return pages;
        }

        private void Navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
            if(_pages == null) {
                _pages = GeneratePages();
            }
            Type pageType = args.IsSettingsInvoked ? null : _pages[(string)args.InvokedItemContainer.Tag];
            if (pageType == null) {
                pageType = typeof(ComingSoon);
            }
            ContentFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }



        private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) {
            if (ContentFrame.CanGoBack) {
                ContentFrame.GoBack();
            }
        }

    }
    
}
