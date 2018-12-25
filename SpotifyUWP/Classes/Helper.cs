using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpotifyUWP {
    public static class Helper {

        public static readonly string Assets = GetAssets().Result;
        public static readonly string Temp = ApplicationData.Current.TemporaryFolder.Path;

        private async static Task<string> GetAssets() => (await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Web")).Path;

        static public string EncodeTo64(string toEncode) {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}
