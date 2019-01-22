using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Duckify {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Search : Page {
        public Search() {
            this.InitializeComponent();
        }

        public ObservableCollection<SearchResult> SongResults { get; set; } = new ObservableCollection<SearchResult>();
        public ObservableCollection<SearchResult> AlbumsResults { get; set; } = new ObservableCollection<SearchResult>();
        public ObservableCollection<SearchResult> PlaylistResults { get; set; } = new ObservableCollection<SearchResult>();


        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            ClearResults();
            string old = SearchBox.Text;
            var results = await Spotify.Client.SearchItemsAsync(SearchBox.Text, SpotifyAPI.Web.Enums.SearchType.All, 5);
            if(SearchBox.Text == old) {
                AddSongResults(results.Tracks?.Items);
                AddPlaylistResults(results.Playlists?.Items);
                AddAlbumResults(results.Albums?.Items);
            }
        }

        private void ClearResults() {
            SongResults.Clear();
            AlbumsResults.Clear();
            PlaylistResults.Clear();
        }

        private void AddSongResults(List<FullTrack> results) {
            results?.ForEach(x => SongResults.Add(new SearchResult(x.Id, x.Name, Helper.BuildListString(x.Artists.Select(y => y.Name)), x.Album.Images[0]?.Url)));
            if(!(SongResults.Count > 0)) {
                var task = AnimationExtensions.Fade(Tracks, 0).StartAsync();
                task.ContinueWith(async(res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Tracks.Visibility = Visibility.Collapsed;
                    });
                });
            } else if(Tracks.Opacity != 1) {
                var task = AnimationExtensions.Fade(Tracks, 1).StartAsync();
                task.ContinueWith(async(res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Tracks.Visibility = Visibility.Visible;
                    });
                });
            }
        }

        private void AddAlbumResults(List<SimpleAlbum> results) {
            results?.ForEach(x => AlbumsResults.Add(new SearchResult(x.Id, x.Name, Helper.BuildListString(x.Artists.Select(y => y.Name)), x?.Images[0]?.Url)));
            if (!(AlbumsResults.Count > 0)) {
                var task = AnimationExtensions.Fade(Albums, 0).StartAsync();
                task.ContinueWith(async (res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Albums.Visibility = Visibility.Collapsed;
                    });
                });
            } else if (Albums.Opacity != 1) {
                var task = AnimationExtensions.Fade(Albums, 1).StartAsync();
                task.ContinueWith(async(res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Albums.Visibility = Visibility.Visible;
                    });
                });
            }
        }

        private void AddPlaylistResults(List<SimplePlaylist> results) {
            results?.ForEach(x => PlaylistResults.Add(new SearchResult(x.Id, x.Name, x.Owner.DisplayName, x.Images[0]?.Url, x.Owner.Id)));
            if (!(PlaylistResults.Count > 0)) {
                var task = AnimationExtensions.Fade(Playlists, 0).StartAsync();
                task.ContinueWith(async(res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Playlists.Visibility = Visibility.Collapsed;
                    });
                });
            } else if (Playlists.Opacity != 1) {
                var task = AnimationExtensions.Fade(Playlists, 1).StartAsync();
                task.ContinueWith(async(res) => {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                        Playlists.Visibility = Visibility.Visible;
                    });
                });
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            Navigation.Navigate(typeof(QueuePlayer), null);
            foreach (SearchResult item in SongsPanel.SelectedItems.ToList()) {
                await Queue.Add(item.Id, "Track", item.UserId);
            }
            foreach (SearchResult item in AlbumsPanel.SelectedItems.ToList()) {
                await Queue.Add(item.Id, "Album", item.UserId);
            }
            foreach (SearchResult item in PlaylistsPanel.SelectedItems.ToList()) {
                await Queue.Add(item.Id, "Playlist", item.UserId);
            }

        }

        private async void MorePlaylists_Tapped(object sender, TappedRoutedEventArgs e){
            var results = await Spotify.Client.SearchItemsAsync(SearchBox.Text, SpotifyAPI.Web.Enums.SearchType.Playlist, 5, PlaylistResults.Count);
            AddPlaylistResults(results.Playlists?.Items);

        }

        private async void MoreAlbums_Tapped(object sender, TappedRoutedEventArgs e) {
            var results = await Spotify.Client.SearchItemsAsync(SearchBox.Text, SpotifyAPI.Web.Enums.SearchType.Album, 5, AlbumsResults.Count);
            AddAlbumResults(results.Albums?.Items);
        }

        private async void MoreSongs_Tapped(object sender, TappedRoutedEventArgs e){
            var results = await Spotify.Client.SearchItemsAsync(SearchBox.Text, SpotifyAPI.Web.Enums.SearchType.Track, 5, SongResults.Count);
            AddSongResults(results.Tracks?.Items);

        }
    }

    public class SearchResult {
        public SearchResult(string id, string name, string author, string imageUrl = null, string userId = null) {
            if(imageUrl != null) {
                ImageUrl = imageUrl;
            }
            UserId = userId;
            Name = name;
            Author = author;
            Id = id;
        }
        public string UserId { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
    }

}
