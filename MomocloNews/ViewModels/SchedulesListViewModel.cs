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
    public class SchedulesListViewModel : ViewModelBase<SchedulesListViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public SchedulesListViewModel() { }

        public SchedulesListViewModel(PhoneApplicationFrame app, INavigator navigator, IMomocloNewsService service, FeedDataContext dataContext)
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

        private bool read = false;
        private int page = Constants.App.BasePage;

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

        private bool _HasNextPage = true;
        public bool HasNextPage
        {
            get { return _HasNextPage; }
            set
            {
                if (_HasNextPage == value) return;
                _HasNextPage = value;
                NotifyPropertyChanged(m => HasNextPage);
            }
        }

        private ObservableCollection<ScheduleItem> _ScheduleItems = new ObservableCollection<ScheduleItem>();
        public ObservableCollection<ScheduleItem> ScheduleItems
        {
            get { return _ScheduleItems; }
            set
            {
                if (_ScheduleItems == value) return;
                _ScheduleItems = value;
                NotifyPropertyChanged(m => ScheduleItems);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand ListSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<LongListSelector>((e) =>
                {
                    if (e.SelectedItem != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/ScheduleDetailPage.xaml", UriKind.Relative), e.SelectedItem);
                        e.SelectedItem = null;
                    }
                }
                );
            }
        }

        public ICommand ListStretchingBottomCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    LoadScheduleItems(false, true);
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void LoadScheduleItems(bool clear, bool next)
        {
            if (IsBusy) return;

            if (clear)
            {
                HasNextPage = true;
                ScheduleItems.Clear();
                page = Constants.App.BasePage;
            }
            else
            {
                if (read)
                {
                    if (!next) return;
                    if (!HasNextPage) return;
                    page++;
                }
                else
                {
                    HasNextPage = true;
                }
            }

            IsBusy = true;

            service.GetScheduleItems(page, LoadScheduleItemsCompleted);
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        protected void LoadScheduleItemsCompleted(MomocloNewsService.GetScheduleItemsResult result, Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;
            read = true;

            if (error != null)
            {
                NotifyError(Localization.AppResources.MainPage_Error_FailedToGetScheduleItems, error);
                return;
            }

            foreach (var item in result.ScheduleItems)
            {
                ScheduleItems.Add(item);
            }
            HasNextPage = result.HasNext;
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