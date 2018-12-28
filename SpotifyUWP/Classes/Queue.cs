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

        public static PlaybackContext CurrentSong { get; set; }

        public static ObservableCollection<QueuedItem> Q { get; set; } = new ObservableCollection<QueuedItem>();

        /// <summary>
        /// Adds anything to queue by it's URI, works for Albums, Playlists and individual tracks
        /// </summary>
        /// <param name="uri">Uri of object to add</param>
        public async static Task Add(string id, string type, string userId) {
            switch (type) {
                case "Track":
                    Q.Add(new QueuedItem(await Spotify.Client.GetTrackAsync(id)));
                    break;
                case "Playlist":
                    var tracks = await Spotify.Client.GetPlaylistTracksAsync(userId, id, "", 100);
                    foreach (var track in tracks.Items) {
                        Q.Add(new QueuedItem(await Spotify.Client.GetTrackAsync(track.Track.Id)));
                    }
                    break;
                case "Album":
                    var albumTracks = await Spotify.Client.GetAlbumTracksAsync(id, 50);
                    foreach (var track in albumTracks.Items) {
                        Q.Add(new QueuedItem(await Spotify.Client.GetTrackAsync(track.Id)));
                    }
                    break;
            }

            if (!(CurrentSong?.IsPlaying ?? false)) {
                await StartPlayback();
            }

        }

        public async static Task StartPlayback() {
            var response = await Spotify.Client.ResumePlaybackAsync((await Spotify.GetDevice()).Id,"", new List<string>() { Q[0].Song.Uri }, 0, 0);
        }

        /// <summary>
        /// Fires SongChanged event and changes song to next song that is in queue
        /// </summary>
        public async static Task Next() {
            if (Q.Count > 1) {
                Q.RemoveAt(0);
                await StartPlayback();

            }

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
            Length = Helper.ConvertMsToReadable(song.DurationMs);
        }
        public QueuedItem(FullTrack song, string addedBy) {
            Likes = 0;
            Song = song;
            AddedBy = addedBy;
            Length = Helper.ConvertMsToReadable(song.DurationMs);
        }

        public string AddedBy { get; set; } = "Admin";
        public FullTrack Song { get; set; }
        public uint Likes { get; set; }
        public string Length { get; set; }

        public string BuildListString(IEnumerable<SimpleArtist> items) {
            var result = "";
            foreach (var item in items) {
                result += item.Name + ", ";
            }
            return result.Remove(result.Length - 2, 2);
        }
    }
}
