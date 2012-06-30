using System;
using MomocloNews.Data;

namespace MomocloNews.Services
{
    public interface IMomocloNewsService
    {
        // Feed
        void GetAllFeedGroupsAndChannels(FeedDataContext dataContext, Action<Exception> callback);
        void GetFeedItems(FeedDataContext dataContext, int[] groupIds, int[] channelIds, int page, Action<MomocloNewsService.GetFeedItemsResult, Exception> callback);

        // Schedule
        void GetScheduleItems(int page, Action<MomocloNewsService.GetScheduleItemsResult, Exception> callback);

        // Notification
        void RegisterNotificationChannel(string version, string langCode, Action<MomocloNewsService.RegisterNotificationChannelResult, Exception> callback);
        void UnregisterNotificationChannel(string uuid, Action<MomocloNewsService.UnregisterNotificationChannelResult, Exception> callback);
        void UpdateNotificationChannel(string uuid, string version, string langCode, int[] channelIds, bool resetUnreads, Action<MomocloNewsService.UpdateNotificationChannelResult, Exception> callback);
    }
}
