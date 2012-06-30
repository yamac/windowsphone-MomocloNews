using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MomocloNews.Data;
using MomocloNews.Navigation;
using MomocloNews.Services;
using SimpleMvvmToolkit;

namespace MomocloNews.ViewModels
{
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public MainPageViewModel() { }

        public MainPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IMomocloNewsService service, FeedDataContext dataContext)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.InitializeCompleted, OnInitializeCompleted);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            if (!IsInDesignMode)
            {
                SendMessage(Constants.MessageTokens.InitializeCompleted, new NotificationEventArgs());
            }
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnInitializeCompleted(object sender, NotificationEventArgs e)
        {
            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                service.UpdateNotificationChannel(uuid, Helpers.AppAttributes.Version, uicc.Name, null, true, UpdateNotificationChannelCompleted);
            }

            LoadPivotItem(3, true);
            LoadPivotItem(0, true);

            ShellTile shellTile = ShellTile.ActiveTiles.First();
            StandardTileData shellTileData = new StandardTileData()
            {
                Title = null,
                BackgroundImage = null,
                Count = 0,
            };
            shellTile.Update(shellTileData);
        }

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

        private ChannelsUpdatesListViewModel _MatomeChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel MatomeChannelsUpdatesListViewModel
        {
            get { return _MatomeChannelsUpdatesListViewModel; }
            set
            {
                if (_MatomeChannelsUpdatesListViewModel == value) return;
                _MatomeChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => MatomeChannelsUpdatesListViewModel);
            }
        }

        private SchedulesListViewModel _SchedulesListViewModel = null;
        public SchedulesListViewModel SchedulesListViewModel
        {
            get { return _SchedulesListViewModel; }
            set
            {
                if (_SchedulesListViewModel == value) return;
                _SchedulesListViewModel = value;
                NotifyPropertyChanged(m => SchedulesListViewModel);
            }
        }

        private ChannelsListViewModel _ChannelsListViewModel = null;
        public ChannelsListViewModel ChannelsListViewModel
        {
            get { return _ChannelsListViewModel; }
            set
            {
                if (_ChannelsListViewModel == value) return;
                _ChannelsListViewModel = value;
                NotifyPropertyChanged(m => ChannelsListViewModel);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand PivotSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    LoadPivotItem(e.SelectedIndex, true);
                }
                );
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    LoadPivotItem(e.SelectedIndex, false);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (e.SelectedItem == null)
                    {
                        return false;
                    }
                    switch (e.SelectedIndex)
                    {
                        case 0:
                            {
                                return !MemberChannelsUpdatesListViewModel.IsBusy;
                            }
                        case 1:
                            {
                                return !MatomeChannelsUpdatesListViewModel.IsBusy;
                            }
                        case 2:
                            {
                                return !SchedulesListViewModel.IsBusy;
                            }
                        case 3:
                            {
                                return !ChannelsListViewModel.IsBusy;
                            }
                        default:
                            return false;
                    }
                }
                );
            }
        }

        public ICommand GotoPreferencesPageCommand
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    NavigationService.Navigate(new Uri("/Views/PreferencesPage.xaml", UriKind.Relative));
                }
                ,
                () =>
                {
                    return !IsBusy;
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        private void LoadPivotItem(int id, bool initializing)
        {
            switch (id)
            {
                case 0:
                {
                    if (MemberChannelsUpdatesListViewModel == null)
                    {
                        MemberChannelsUpdatesListViewModel =
                            new ChannelsUpdatesListViewModel(app, navigator, service, dataContext, new int[] { 1 }, null);
                        MemberChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;
                        MemberChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                    }
                    else if (!initializing)
                    {
                        MemberChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                    }
                    break;
                }
                case 1:
                {
                    if (MatomeChannelsUpdatesListViewModel == null)
                    {
                        MatomeChannelsUpdatesListViewModel =
                            new ChannelsUpdatesListViewModel(app, navigator, service, dataContext, new int[] { 3 }, null);
                        MatomeChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;
                        MatomeChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                    }
                    else if (!initializing)
                    {
                        MatomeChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                    }
                    break;
                }
                case 2:
                {
                    if (SchedulesListViewModel == null)
                    {
                        SchedulesListViewModel =
                            new SchedulesListViewModel(app, navigator, service, dataContext);
                        SchedulesListViewModel.ErrorNotice += OnErrorNotice;
                        SchedulesListViewModel.LoadScheduleItems(true, false);
                    }
                    else if (!initializing)
                    {
                        SchedulesListViewModel.LoadScheduleItems(true, false);
                    }
                    break;
                }
                case 3:
                {
                    if (ChannelsListViewModel == null)
                    {
                        ChannelsListViewModel = new ChannelsListViewModel(app, navigator, service, dataContext);
                        ChannelsListViewModel.ErrorNotice += OnErrorNotice;
                        ChannelsListViewModel.LoadAllFeedGroupsAndChannels();
                    }
                    else if (!initializing)
                    {
                        ChannelsListViewModel.LoadAllFeedGroupsAndChannels();
                    }
                    break;
                }
            }
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        void UpdateNotificationChannelCompleted(MomocloNewsService.UpdateNotificationChannelResult result, Exception error)
        {
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

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            Notify(ErrorNotice, e);
        }

        #endregion
    }
}