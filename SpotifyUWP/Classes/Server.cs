using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HttpListener = System.Net.Http.HttpListener;

namespace SpotifyUWP{
    class Server{
        public static HttpListener Listener;

        public static void Init() {
            Listener = new HttpListener(Helper.GetMyIP(), 80);
            Listener.Request += async (s, ev) => await HandleRequest(ev);
            Listener.Start();
        }

        public static async Task HandleRequest(HttpListenerRequestEventArgs args) {

            await args.Response.WriteContentAsync(Queue.CurrentSong.Item.Name);
            args.Response.Close();
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
