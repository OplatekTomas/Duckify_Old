using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace SpotifyUWP {
    public static class Helper {

        public static readonly string Assets = GetAssets().Result;
        public static readonly string Temp = ApplicationData.Current.TemporaryFolder.Path;

        private async static Task<string> GetAssets() => (await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Web")).Path;

        static public string EncodeTo64(string toEncode) {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string BuildListString(IEnumerable<string> items) {
            var result = "";
            foreach(var item in items) {
                result += item + ", ";
            }
            return result.Remove(result.Length - 2, 2);
        }

        public static string GetMyIPString() {
            return GetMyIP().ToString();
        }


        public static IPAddress GetMyIP() {
            IPAddress localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address;
            }
            return localIP;
        }
    }
}
