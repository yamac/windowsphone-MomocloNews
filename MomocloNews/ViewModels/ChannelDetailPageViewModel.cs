using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MomocloNews.Data;
using MomocloNews.Navigation;
using MomocloNews.Services;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Microsoft.Phone.Shell;
using System.Linq;

namespace MomocloNews.ViewModels
{
    public class ChannelDetailPageViewModel : ViewModelBase<ChannelDetailPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelDetailPageViewModel() { }

        public ChannelDetailPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IMomocloNewsService service, FeedDataContext dataContext)
        {
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            if (!IsInDesignMode)
            {
                FeedChannel = NavigationService.NavigationArgs as FeedChannel;
                MemberChannelsUpdatesListViewModel =
                    new ChannelsUpdatesListViewModel(app, navigator, service, dataContext, null, new int[] { FeedChannel.Id });
                MemberChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;
                MemberChannelsUpdatesListViewModel.LoadFeedItems(true, false);
            }
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

        private bool initialized = false;

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

        private FeedChannel _FeedChannel = null;
        public FeedChannel FeedChannel
        {
            get { return _FeedChannel; }
            set
            {
                if (_FeedChannel == value) return;
                _FeedChannel = value;
                NotifyPropertyChanged(m => FeedChannel);
            }
        }

        private int _PivotItemSelectedIndex = -1;
        public int PivotItemSelectedIndex
        {
            get { return _PivotItemSelectedIndex; }
            set
            {
                if (_PivotItemSelectedIndex == value) return;
                _PivotItemSelectedIndex = value;
                NotifyPropertyChanged(m => PivotItemSelectedIndex);
            }
        }

        private ChannelsUpdatesListViewModel _MemberChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel MemberChannelsUpdatesListViewModel
        {
            get { return _MemberChannelsUpdatesListViewModel; }
            set
            {
                if (_MemberChannelsUpdatesListViewModel == value) return;
                _MemberChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => MemberChannelsUpdatesListViewModel);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        /*
        public ICommand PivotSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    (e.SelectedItem as MainPageMembersUpdatesListViewModel).LoadFeedItems(false, false);
                }
                );
            }
        }
        */

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    (e.SelectedItem as ChannelsUpdatesListViewModel).LoadFeedItems(true, false);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (!initialized)
                    {
                        return true;
                    }
                    if (e.SelectedItem == null)
                    {
                        return false;
                    }
                    return !(e.SelectedItem as ChannelsUpdatesListViewModel).IsBusy;
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        #endregion

        #region Helpers
        /***********
         * Helpers *
         ***********/

        private void NotifyError(string message, Exception error)
        {
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            Notify(ErrorNotice, e);
        }

        #endregion
    }
}