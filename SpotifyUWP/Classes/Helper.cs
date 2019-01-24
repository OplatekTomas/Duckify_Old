using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Duckify {
    public static class Helper {

        public static readonly string AssetsWeb = GetAssets().Result;
        public static readonly string Temp = ApplicationData.Current.TemporaryFolder.Path;

        private async static Task<string> GetAssets() => (await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Web")).Path;

        static public string EncodeTo64(string toEncode) {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string BuildListString(IEnumerable<string> items) {
            var result = "";
            foreach (var item in items) {
                result += item + ", ";
            }
            return result.Remove(result.Length - 2, 2);
        }

        public static string GetMyIPString() {
            return GetMyIP().ToString();
        }

        public static string ConvertMsToReadable(double ms) {
            TimeSpan ts = TimeSpan.FromMilliseconds(ms);
            return ts.ToString(@"mm\:ss");
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

        /// <summary>
        /// Tries to convert any single type to any single other type by comparing its properties.
        /// Takes properties with same name and assings their values
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="obj">Object of type to convert from</param>
        /// <returns>New object of type T</returns>
        public static T ConvertType<T>(object obj) {
            Type origType = obj.GetType();
            Type newType = typeof(T);
            var origProps = origType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var newProps = newType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var same = origProps.Intersect(newProps).ToList();
            var res = origProps.Select(x => (x.Name, x.PropertyType)).ToDictionary(x => x.Name, x => x.PropertyType)
                .Intersect(newProps.Select(x => (x.Name, x.PropertyType)).ToDictionary(x => x.Name, x => x.PropertyType)).ToDictionary(x => x.Key, x => x.Value);
            var newObj = Activator.CreateInstance(typeof(T));
            foreach (var item in res) {
                newProps.First(x => x.Name == item.Key).SetValue(newObj, origProps.First(x => x.Name == item.Key).GetValue(obj));
            }
            return (T)newObj;
        }
    }
}
