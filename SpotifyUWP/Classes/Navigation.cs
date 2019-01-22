using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Duckify{
    class Navigation {

        private static Frame _view = Init();

        private static Frame Init() {
            return ((MainPage)((Frame)Window.Current.Content).Content).FindName("ContentFrame") as Frame;
        }

        public static void Navigate(Type pageType, object args) {
            _view.Navigate(pageType, args);
        }

    }
}
