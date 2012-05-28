using System;
using System.Windows;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Coding4Fun.Phone.Controls;
using MomocloNews.ViewModels;
using Microsoft.Phone.Tasks;

namespace MomocloNews.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            var vm = (MainPageViewModel)DataContext;
            vm.ErrorNotice += OnErrorNotice;
        }

        public void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            System.Diagnostics.Debug.WriteLine("OnErrorNotify:" + (e.Data != null ? e.Data.ToString() : "null"));
            var toast = new ToastPrompt
            {
                Message = (e.Message != null ? e.Message : null),
                TextWrapping = TextWrapping.Wrap,
            };
            toast.Show();
        }

        private void HubTile1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.momoclo.net/member/takagi/index.html", UriKind.Absolute);
            task.Show();
            */

           MomocloNews.Navigation.NavigationService.Navigate(new Uri("/Views/ChannelDetailPage.xaml", UriKind.Relative), "");

        }

        private void HubTile2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.momoclo.net/member/momota/index.html", UriKind.Absolute);
            task.Show();
        }

        private void HubTile3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.momoclo.net/member/tamai/index.html", UriKind.Absolute);
            task.Show();
        }

        private void HubTile4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.momoclo.net/member/sasaki/index.html", UriKind.Absolute);
            task.Show();
        }

        private void HubTile5_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.momoclo.net/member/ariyasu/index.html", UriKind.Absolute);
            task.Show();
        }
    }
}