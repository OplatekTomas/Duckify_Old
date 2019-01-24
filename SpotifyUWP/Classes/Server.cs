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

namespace Duckify{
    class Server{
        public static HttpListener Listener;

        public static void Init() {
            //Listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 5850);
            Listener = new HttpListener(Helper.GetMyIP(), 5850);

            Listener.Request += async (s, ev) => await HandleRequest(ev);
            Listener.Start();
        }

        public static async Task HandleRequest(HttpListenerRequestEventArgs args) {
            var req = args.Request;
            Stream response = null;
            string type = "";
            if (args.Request.HttpMethod == "GET") {
                if (req.Url.LocalPath.StartsWith("/api/")) {

                } else {
                    (Stream response, string type) res = await OpenPathToStream(req.Url.LocalPath);
                    response = res.response;
                    type = res.type;
                }

            } else if (args.Request.HttpMethod == "POST") {
               
            } else {
                args.Response.MethodNotAllowed();
            }
            if (response != null) {
                await HandleSuccess(args, response,type);
            } else {
                 HandleNotFound(args);
            }
        }

        private static async Task<(Stream response,string type)> OpenPathToStream(string path) {
            if (path == "/") {
                path = "/index.html";
            }
            if (File.Exists(Helper.AssetsWeb + "\\index" + path)) {
                return (File.OpenRead(Helper.AssetsWeb + "\\index" + path), Path.GetExtension(Helper.AssetsWeb + "\\index" + path));
            } else {
                return (null,null);
            }
        }

        private static void HandleNotFound(HttpListenerRequestEventArgs context) {
            context.Response.NotFound();
            context.Response.Close();
        }

        private static async Task HandleSuccess(HttpListenerRequestEventArgs context, Stream input, string type) {
            context.Response.OutputStream.Position = 0;
            await input.CopyToAsync(context.Response.OutputStream);
            context.Response.Headers.Add("Content-Type", MimeHelper.Map[type]);
            context.Response.StatusCode = 200;
            input.Close();
            context.Response.Close();
        }

        public static async Task WriteToResponseStream(string text, HttpListenerRequestEventArgs args) {
            using (StreamWriter writer = new StreamWriter(args.Response.OutputStream)) {
                args.Response.OutputStream.Position = 0;
                await writer.WriteAsync(text);
                await writer.FlushAsync();
            }
        }
       

    }
}
