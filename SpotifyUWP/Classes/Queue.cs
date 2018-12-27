using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyUWP
{
    class Queue{
        public static EventHandler SongChanged { get; set; }

        public static PlaybackContext CurrentSong { get; set; }

        public static ObservableCollection<QueuedItem> Q { get; set; } = new ObservableCollection<QueuedItem>();

        /// <summary>
        /// Adds anything to queue by it's URI, works for Albums, Playlists and individual tracks
        /// </summary>
        /// <param name="uri">Uri of object to add</param>
        public async static Task Add(string id, string type) {
            switch (type) {
                case "Track":
                    Q.Add(new QueuedItem(await Spotify.Client.GetTrackAsync(id)));
                    break;
            }

            if (!(CurrentSong?.IsPlaying ?? false)) {
                await StartPlayback();
            }

        }

        public async static Task StartPlayback() {
            var response = await Spotify.Client.ResumePlaybackAsync(Spotify.DeviceId,"", new List<string>() { Q[0].Song.Uri }, 0, 0);
            SongChanged.Invoke(Q[0].Song, new EventArgs());
        }

        /// <summary>
        /// Fires SongChanged event and changes song to next song that is in queue
        /// </summary>
        public static void Next() {
            
        }

        /// <summary>
        /// Removes song from a queue by its id
        /// </summary>
        /// <param name="id">Id of a song</param>
        public static void Remove(string id) {

        }

    }

    public class QueuedItem {
        public QueuedItem(FullTrack song) {
            Likes = 0;
            Song = song;
        }
        public QueuedItem(FullTrack song, string addedBy) {
            Likes = 0;
            Song = song;
            AddedBy = addedBy;
        }

        public string AddedBy { get; set; } = "Admin";
        public FullTrack Song { get; set; }
        public uint Likes { get; set; }

        public string BuildListString(IEnumerable<SimpleArtist> items) {
            var result = "";
            foreach (var item in items) {
                result += item.Name + ", ";
            }
            return result.Remove(result.Length - 2, 2);
        }
    }
}
