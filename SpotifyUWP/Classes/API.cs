using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Duckify {
    class API {


        public static async Task Handle(string path, HttpListenerResponse response) {
            var pathParts = path.Split("/");
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
                default:
                    //Again. Someones bullshitting me.
                    SendBS(response);
                    return;
            }
            WriteTextReponse(responseText, response);
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

