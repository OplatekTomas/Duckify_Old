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

namespace SpotifyUWP {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Search : Page {
        public Search() {
            this.InitializeComponent();
        }

        public ObservableCollection<SearchResult> Results { get; set; } = new ObservableCollection<SearchResult>();


        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Results.Clear();
            string old = SearchBox.Text;
            var results = await Spotify.Client.SearchItemsAsync(SearchBox.Text, SpotifyAPI.Web.Enums.SearchType.All, 5);
            if(SearchBox.Text == old) {
                results.Tracks?.Items.ForEach(x => Results.Add(new SearchResult(x.Id, x.Name, Helper.BuildListString(x.Artists.Select(y => y.Name)), "Track", x.Album.Images[0]?.Url)));
                results.Albums?.Items.ForEach(x => Results.Add(new SearchResult(x.Id, x.Name, Helper.BuildListString(x.Artists.Select(y => y.Name)), "Album", x.Images[0]?.Url)));
                results.Playlists?.Items.ForEach(x => Results.Add(new SearchResult(x.Id, x.Name, x.Owner.DisplayName, "Playlist", x.Images[0]?.Url)));
            }


        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            var items = SearchResultsGrid.SelectedItems.ToList();
            foreach(SearchResult item in items) {
                await Queue.Add(item.Id,item.Type);
            }
            Navigation.Navigate(typeof(QueuePlayer), null);
        }
    }

    public class SearchResult {
        public SearchResult(string id,string name, string author, string type, string url = null) {
            if(url != null) {
                ImageUrl = url;
            }
            Name = name;
            Author = author;
            Type = type;
            Id = id;
        }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
    }

}
