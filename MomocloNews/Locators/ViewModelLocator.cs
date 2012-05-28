using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using MomocloNews.Services;
using MomocloNews.ViewModels;
using MomocloNews.Data;

namespace MomocloNews.Locators
{
    public class ViewModelLocator
    {
        private PhoneApplicationFrame TheApp
        {
            get
            {
                return App.Current.RootVisual as PhoneApplicationFrame;
            }
        }

        private INavigator _TheNavigator;
        private INavigator TheNavigator
        {
            get
            {
                if (_TheNavigator == null)
                {
                    _TheNavigator = new Navigator();
                }
                return _TheNavigator;
            }
        }

        private static IMomocloNewsService _TheMomocloNewsService;
        private IMomocloNewsService TheMomocloNewsService
        {
            get
            {
                if (_TheMomocloNewsService == null)
                {
                    _TheMomocloNewsService = new MomocloNewsService();
                }
                return _TheMomocloNewsService;
            }
        }

        private static FeedDataContext _TheFeedDataContext;
        private FeedDataContext TheFeedDataContext
        {
            get
            {
                if (_TheFeedDataContext == null)
                {
                    _TheFeedDataContext = new FeedDataContext();
                }
                return _TheFeedDataContext;
            }
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return new MainPageViewModel(TheApp, TheNavigator, TheMomocloNewsService, TheFeedDataContext);
            }
        }

        public ChannelDetailPageViewModel ChannelDetailPageViewModel
        {
            get
            {
                return new ChannelDetailPageViewModel(TheApp, TheNavigator, TheMomocloNewsService, TheFeedDataContext);
            }
        }

        public WebPageViewModel WebPageViewModel
        {
            get
            {
                return new WebPageViewModel();
            }
        }

        public PreferencesPageViewModel PreferencesPageViewModel
        {
            get
            {
                return new PreferencesPageViewModel(TheApp, TheNavigator, TheMomocloNewsService, TheFeedDataContext);
            }
        }
    }
}