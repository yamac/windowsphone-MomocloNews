using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MomocloNews.Data;
using MomocloNews.Navigation;
using MomocloNews.Services;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using System.Globalization;
using System.Threading;
using Microsoft.Phone.Tasks;
using System.ComponentModel;

namespace MomocloNews.ViewModels
{
    public class ChannelsListViewModel : ViewModelBase<ChannelsListViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelsListViewModel() { }

        public ChannelsListViewModel(PhoneApplicationFrame app, INavigator navigator, IMomocloNewsService service, FeedDataContext dataContext)
        {
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Services
        /************
         * Services *
         ************/

        PhoneApplicationFrame app;
        INavigator navigator;
        IMomocloNewsService service;
        FeedDataContext dataContext;

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (_IsBusy == value) return;
                _IsBusy = value;
                NotifyPropertyChanged(m => IsBusy);
            }
        }

        private ObservableCollection<FeedChannel> _FeedChannels = null;
        public ObservableCollection<FeedChannel> FeedChannels
        {
            get { return _FeedChannels; }
            set
            {
                if (_FeedChannels == value) return;
                _FeedChannels = value;
                NotifyPropertyChanged(m => FeedChannels);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand TheMembersListSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<ListBox>((e) =>
                {
                    if (e.SelectedItem != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/ChannelDetailPage.xaml", UriKind.Relative), e.SelectedItem);
                        e.SelectedItem = null;
                    }
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void LoadAllFeedGroupsAndChannels()
        {
            if (IsBusy) return;

            IsBusy = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork +=
                (s, e) =>
                {
                    try
                    {
                        FeedChannel[] channels =
                        (
                            from channel
                            in dataContext.FeedChannels
                            where channel.FeedGroupId == 1 && channel.Status == 1
                            orderby channel.ListOrder
                            ascending
                            select channel
                        ).ToArray();
                        if (channels.Count() > 0)
                        {
                            FeedChannels = new ObservableCollection<FeedChannel>(channels);
                        }
                    }
                    catch
                    {
                    }

                    service.GetAllFeedGroupsAndChannels(dataContext, LoadAllFeedGroupsAndChannelsCompleted);
                };
            bw.RunWorkerAsync();
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        protected void LoadAllFeedGroupsAndChannelsCompleted(Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;

            FeedChannel[] channels = 
            (
                from channel
                in dataContext.FeedChannels
                where channel.FeedGroupId == 1 && channel.Status == 1
                orderby channel.ListOrder
                ascending select channel
            ).ToArray();
            if (error != null && channels.Count() == 0)
            {
                NotifyError(Localization.AppResources.MainPage_Error_FailedToGetAllFeedGroupsAndChannels, error);
                return;
            }
            if (FeedChannels == null)
            {
                FeedChannels = new ObservableCollection<FeedChannel>(channels);
            }
        }

        #endregion

        #region Helpers
        /***********
         * Helpers *
         ***********/

        private void NotifyError(string message, Exception error)
        {
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}