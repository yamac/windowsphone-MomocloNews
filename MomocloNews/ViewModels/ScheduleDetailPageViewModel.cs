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
    public class ScheduleDetailPageViewModel : ViewModelBase<ScheduleDetailPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ScheduleDetailPageViewModel() { }

        public ScheduleDetailPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IMomocloNewsService service, FeedDataContext dataContext)
        {
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            if (!IsInDesignMode)
            {
                ScheduleItem = NavigationService.NavigationArgs as ScheduleItem;
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

        private ScheduleItem _ScheduleItem = null;
        public ScheduleItem ScheduleItem
        {
            get { return _ScheduleItem; }
            set
            {
                if (_ScheduleItem == value) return;
                _ScheduleItem = value;
                NotifyPropertyChanged(m => ScheduleItem);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/


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