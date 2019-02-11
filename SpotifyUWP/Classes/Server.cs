using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using HttpListener = System.Net.Http.HttpListener;
using HttpListenerResponse = System.Net.Http.HttpListenerResponse;

namespace Duckify {
    class Server {
        public static HttpListener Listener;
        private static string FileDirectory = Helper.AssetsWeb + "\\Web";

        /// <summary>
        /// Initalize and start the Listener property. Server starts on local network address and port 5850
        /// </summary>
        public static void Init() {
            //Listener = new HttpListener(Helper.GetMyIP(), 5850);
            Listener = new HttpListener(Helper.GetMyIP(), 5850);
            Listener.Request += async (s, ev) => await HandleRequest(ev);
            Listener.Start();
        }

        private static async Task HandleRequest(HttpListenerRequestEventArgs args) {
            var request = args.Request;
            var response = args.Response;
            //
            // TODO: Handle Auth
            //

            //Check if request is for API or not.
            bool isApi = request.Url.LocalPath.StartsWith("/api");
            if (isApi) {
                await API.Handle(request.Url, args.Response);
            } else {
                await GetFileResponse(request.Url.LocalPath, args.Response);
            }

            //By now response has been built so we can close it.
            args.Response.Close();
        }

        private async static Task GetFileResponse(string path, HttpListenerResponse response) {
            if (path == "/") {
                path += "index.html";
            }
            path = FileDirectory + path.Replace("/", "\\");
            if (!File.Exists(path)) {
                response.NotFound();
                return;
            }
            response.Headers.Add("Content-Type", MimeHelper.GetMime(Path.GetExtension(path)));
            response.OutputStream.Position = 0;
            var file = File.OpenRead(path);
            await file.CopyToAsync(response.OutputStream);
            file.Close();
            response.StatusCode = 200;
        }
    }
}
