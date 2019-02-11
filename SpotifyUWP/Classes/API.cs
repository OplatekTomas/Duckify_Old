using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Duckify {
    class API {

        public static async Task Handle(Uri path, HttpListenerResponse response) {
            var pathParts = path.LocalPath.Split("/");
            string responseText = "";
            //This means someones fucking around with the API and hasn't queried anything. Lets give him some fun response.
            if (pathParts.Length == 2) {
                SendBS(response);
                return;
            }
            switch (pathParts[2]) {
                case "queue":
                    responseText = BuildQueueResponse();
                    break;
                case "search":
                    responseText = await BuildSearchResponse(path.Query.Remove(0, 1));
                    if (responseText == null) {
                        response.NotFound();
                        return;
                    }
                    break;
                case "like":
                    responseText = await BuildLikeResponse(path.Query.Remove(0, 1));
                    break;
                default:
                    //Again. Someones bullshitting me.
                    SendBS(response);
                    return;
            }
            WriteTextReponse(responseText, response);
        }

        public static async Task<string> BuildLikeResponse(string query) {
            var parts = ParseQuery(query);
            if (!parts.ContainsKey("songId")) {
                return null;
            }
            string id = parts["songId"];
            if (!Queue.Q.Select(x => x.Song.Id).Contains(id)) {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>{
                    await Queue.Add(id, "Track", "");
                });

            } else {
                for (int i = 0; i < Queue.Q.Count; i++) {
                    if (Queue.Q[i].Song.Id == id) {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                            Queue.Q[i].Likes++;
                        });
                    }
                }
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () => {
                Queue.Q.BubbleSort();
                
            });
            return "";
        }

        public static async Task<string> BuildSearchResponse(string query) {
            var parts = ParseQuery(query);
            if (!parts.ContainsKey("query")) {
                return null;
            }
            var results = await Spotify.Client.SearchItemsAsync(parts["query"], SpotifyAPI.Web.Enums.SearchType.Track, 15);
            var simplifiedResults = new List<Song>();
            if(results.Tracks == null) {
                return JsonConvert.SerializeObject(simplifiedResults);
            }
            foreach (var result in results.Tracks.Items) {
                var song = new Song() {
                    artistName = Helper.BuildListString(result.Artists.Select(x => x.Name)),
                    songName = result.Name,
                    coverUrl = result.Album.Images.Select(x => (x.Width, x)).Min().Item2.Url,
                    songLength = Helper.ConvertMsToReadable(result.DurationMs),
                    songId = result.Id
                };
                simplifiedResults.Add(song);
            }
            return JsonConvert.SerializeObject(simplifiedResults);
        }

        public static string BuildQueueResponse() {
            var queueTemp = Queue.Q;
            var result = new List<Song>();
            foreach (var item in queueTemp) {
                var song = new Song() {
                    artistName = Helper.BuildListString(item.Song.Artists.Select(x => x.Name)),
                    songName = item.Song.Name,
                    coverUrl = item.Song.Album.Images.Select(x => (x.Width, x)).Min().Item2.Url,
                    songLength = item.Length,
                    numberOfLikes = item.Likes,
                    songId = item.Song.Id
                };
                result.Add(song);
            }
            return JsonConvert.SerializeObject(result);
        }

        public static Dictionary<string, string> ParseQuery(string query) {
            var parts = query.Split("&");
            var result = new Dictionary<string, string>();
            foreach (var part in parts) {
                var splitQuery = part.Split("=", 2);
                if (!result.ContainsKey(splitQuery[0]) && splitQuery.Length > 1) {
                    result.Add(splitQuery[0], splitQuery[1]);
                }
            }
            return result;
        }

        /// <summary>
        /// Sends predefined string that tells sneaky people to fuck off.
        /// </summary>
        private static void SendBS(HttpListenerResponse response) {
            WriteTextReponse("{\"apiInfo\":\"If someone is trying to brake the damn API or find some backdoor to it please don't.\"}", response);
        }

        private static void WriteTextReponse(string text, HttpListenerResponse response) {
            response.OutputStream.Position = 0;
            StreamWriter sr = new StreamWriter(response.OutputStream);
            sr.Write(text);
            sr.Flush();
            response.Headers.Add("Content-Type", "application/json");
            response.StatusCode = 200;
        }


        class Song {
            public string songId;
            public string artistName;
            public string songName;
            public uint numberOfLikes;
            public bool likedByUser;
            public string songLength;
            public string coverUrl;
        }
    }
}

