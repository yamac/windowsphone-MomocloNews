using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using MomocloNews.Data;
using Microsoft.Phone.Reactive;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Phone.Notification;
using System.Runtime.Serialization;
using System.Text;

namespace MomocloNews.Services
{
    public class MomocloNewsService : IMomocloNewsService
    {
        private static class API
        {
#if DEBUG
            private const string Base = "http://apid.yamac.net/momoclo_news/v1.0/";
            private const string SecureBase = "https://secure.yamac.net/apid/momoclo_news/v1.0/";
#else
            private const string Base = "http://api.yamac.net/momoclo_news/v1.0/";
            private const string SecureBase = "https://secure.yamac.net/api/momoclo_news/v1.0/";
#endif
            private const string FeedBase = Base + "feed/";
            private const string DeviceBase = SecureBase + "device/";
            public const string FeedGroups = FeedBase + "groups";
            public const string FeedChannels = FeedBase + "channels";
            public const string FeedItems = FeedBase + "items";
            public const string DeviceRegister = DeviceBase + "register";
            public const string DeviceUnregister = DeviceBase + "unregister";
            public const string DeviceUpdate = DeviceBase + "update";
        }

        private static class Notification
        {
            public const string ChannelName = "MomocloNewsUpdates";
        }

        public MomocloNewsService()
        {
        }

        public void GetAllFeedGroupsAndChannels(FeedDataContext dataContext, Action<Exception> callback)
        {
            var groupsReq = WebRequest.CreateHttp(API.FeedGroups);
            groupsReq.UserAgent = Constants.Net.UserAgent;
            groupsReq.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            var channelsReq = WebRequest.CreateHttp(API.FeedChannels);
            channelsReq.UserAgent = Constants.Net.UserAgent;
            channelsReq.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

            var groupsObs =
                Observable
                .FromAsyncPattern<WebResponse>(groupsReq.BeginGetResponse, groupsReq.EndGetResponse)()
                .Select
                (
                    res =>
                    {
                        // ストリームを取得
                        Stream stream = res.GetResponseStream();
                        if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                        {
                            stream = new GZipInputStream(stream);
                        }

                        // シリアライズ
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FeedGroup[]));
                        var groups = (FeedGroup[])serializer.ReadObject(stream);

                        // データ更新
                        var existsGroupIds = (from theGroup in groups select theGroup.Id).Intersect(from theGroup in dataContext.FeedGroups select theGroup.Id);
                        var existsGroups = from theGroup in groups from existsGroupId in existsGroupIds where theGroup.Id == existsGroupId select theGroup;
                        foreach (var existsGroup in existsGroups)
                        {
                            var oldGroup = dataContext.FeedGroups.SingleOrDefault(theGroup => theGroup.Id == existsGroup.Id && theGroup.UpdatedAt < existsGroup.UpdatedAt) ?? null;
                            if (oldGroup != null)
                            {
                                System.Diagnostics.Debug.WriteLine("既存グループ(更新あり):" + existsGroup.Id + "," + existsGroup.Title);
                                oldGroup.Title = existsGroup.Title;
                                oldGroup.Status = existsGroup.Status;
                                oldGroup.UpdatedAt = existsGroup.UpdatedAt;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("既存グループ(更新なし):" + existsGroup.Id + "," + existsGroup.Title);
                            }
                        }

                        var newGroupIds = (from theGroup in groups select theGroup.Id).Except(from theGroup in dataContext.FeedGroups select theGroup.Id);
                        var newGroups = from theGroup in groups from newGroupId in newGroupIds where theGroup.Id == newGroupId select theGroup;
                        foreach (var newGroup in newGroups)
                        {
                            System.Diagnostics.Debug.WriteLine("新規グループ:" + newGroup.Id + "," + newGroup.Title);
                            dataContext.FeedGroups.InsertOnSubmit(newGroup);
                        }

                        // ストリームを閉じる
                        stream.Close();

                        return 1;
                    }
                );

            var channelsObs =
                Observable
                .FromAsyncPattern<WebResponse>(channelsReq.BeginGetResponse, channelsReq.EndGetResponse)()
                .Select
                (
                    res =>
                    {
                        // ストリームを取得
                        Stream stream = res.GetResponseStream();
                        if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                        {
                            stream = new GZipInputStream(stream);
                        }

                        // シリアライズ
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FeedChannel[]));
                        var channels = (FeedChannel[])serializer.ReadObject(stream);

                        // データ更新
                        var existsChannelIds = (from channel in channels select channel.Id).Intersect(from channel in dataContext.FeedChannels select channel.Id);
                        var existsChannels = from channel in channels from existsChannelId in existsChannelIds where channel.Id == existsChannelId select channel;
                        foreach (var existsChannel in existsChannels)
                        {
                            var oldChannel = dataContext.FeedChannels.SingleOrDefault(channel => channel.Id == existsChannel.Id && channel.UpdatedAt < existsChannel.UpdatedAt) ?? null;
                            if (oldChannel != null)
                            {
                                System.Diagnostics.Debug.WriteLine("既存チャンネル(更新あり):" + existsChannel.Id + "," + existsChannel.Title);
                                oldChannel.FeedGroupId = existsChannel.FeedGroupId;
                                oldChannel.FeedLink = existsChannel.FeedLink;
                                oldChannel.Link = existsChannel.Link;
                                oldChannel.Title = existsChannel.Title;
                                oldChannel.FeedItemLastTitle = existsChannel.FeedItemLastTitle;
                                oldChannel.FeedItemLastPublishedAt = existsChannel.FeedItemLastPublishedAt;
                                oldChannel.ListOrder = existsChannel.ListOrder;
                                oldChannel.AccentColor = existsChannel.AccentColor;
                                oldChannel.AuthorName = existsChannel.AuthorName;
                                oldChannel.AuthorNickName = existsChannel.AuthorNickName;
                                oldChannel.ProfileLink = existsChannel.ProfileLink;
                                oldChannel.Image = existsChannel.Image;
                                oldChannel.Status = existsChannel.Status;
                                oldChannel.UpdatedAt = existsChannel.UpdatedAt;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("既存チャンネル(更新なし):" + existsChannel.Id + "," + existsChannel.Title);
                            }
                        }

                        var newChannelIds = (from channel in channels select channel.Id).Except(from channel in dataContext.FeedChannels select channel.Id);
                        var newChannels = from channel in channels from newChannelId in newChannelIds where channel.Id == newChannelId select channel;
                        foreach (var newChannel in newChannels)
                        {
                            System.Diagnostics.Debug.WriteLine("新規チャンネル:" + newChannel.Id + "," + newChannel.Title);
                            dataContext.FeedChannels.InsertOnSubmit(newChannel);
                        }

                        // ストリームを閉じる
                        stream.Close();

                        // 結果
                        return 1;
                    }
                );

            groupsObs.SelectMany(a => channelsObs)
            .Subscribe
            (
                _ =>
                {
                    // サブミット
                    dataContext.SubmitChanges();
                    callback(null);
                }
                ,
                e =>
                {
                    callback(e);
                }
            );
        }

        public class GetFeedItemsResult
        {
            public FeedItem[] FeedItems { get; private set; }
            public bool HasNext { get; private set; }

            public GetFeedItemsResult(FeedItem[] items, bool hasNext)
            {
                FeedItems = items;
                HasNext = hasNext;
            }
        }

        public void GetFeedItems(FeedDataContext dataContext, int[] groupIds, int[] channelIds, int page, Action<GetFeedItemsResult, Exception> callback)
        {
            bool hasParams = false;
            string uri = API.FeedItems;
            if (groupIds != null)
            {
                uri += (hasParams ? "&" : "?") + "group_id=" + string.Join(",", groupIds);
                hasParams = true;
            }
            if (channelIds != null)
            {
                uri += (hasParams ? "&" : "?") + "channel_id=" + string.Join(",", channelIds);
                hasParams = true;
            }
            uri += (hasParams ? "&" : "?") + "rows=" + Constants.App.ItemsPerPage + "&page=" + page;
            //System.Diagnostics.Debug.WriteLine("GetFeedItems:" + uri);
            var req = WebRequest.CreateHttp(uri);
            req.UserAgent = Constants.Net.UserAgent;
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

            Observable
            .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
            .Invoke()
            .Select<WebResponse, GetFeedItemsResult>
            (
                res =>
                {
                    // ストリームを取得
                    Stream stream = res.GetResponseStream();
                    if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                    {
                        stream = new GZipInputStream(stream);
                    }

                    // シリアライズ
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FeedItem[]));
                    var items = (FeedItem[])serializer.ReadObject(stream);

                    // ストリームを閉じる
                    stream.Close();

                    // 結果
                    var result = new GetFeedItemsResult(items, (items.Count() == Constants.App.ItemsPerPage) && (page < Constants.App.MaxPage));

                    return result;
                }
            )
            .ObserveOnDispatcher()
            .Subscribe
            (
                s =>
                {
                    callback(s, null);
                },
                e =>
                {
                    callback(null, e);
                }
            );
        }

        [DataContract]
        public class RegisterNotificationChannelResult
        {
            [DataContract]
            public class _Response
            {
                [DataMember(Name = "uuid")]
                public string Uuid { get; set; }
            }

            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }

            [DataMember(Name = "response")]
            public _Response Response { get; set; }
        }

        public void RegisterNotificationChannel(string version, string langCode, Action<RegisterNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("RegisterNotificationChannel");
            bool isNewChannel = false;

            HttpNotificationChannel notificationChannel;
            notificationChannel = HttpNotificationChannel.Find(Notification.ChannelName);
            if (notificationChannel != null)
            {
                notificationChannel.Close();
            }

            isNewChannel = true;
            notificationChannel = new HttpNotificationChannel(Notification.ChannelName);

            notificationChannel.ConnectionStatusChanged += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ConnectionStatusChanged:" + e.ConnectionStatus.ToString());
            };

            notificationChannel.ErrorOccurred += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ErrorOccurred:" + e.Message.ToString());
            };

            notificationChannel.HttpNotificationReceived += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_HttpNotificationReceived:" + e.Notification.ToString());
            };

            notificationChannel.ChannelUriUpdated += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ChannelUriUpdated:" + e.ChannelUri.ToString());
                string uri = isNewChannel ? API.DeviceRegister : API.DeviceUpdate;
                System.Diagnostics.Debug.WriteLine(uri);
                string postDataStr;
                postDataStr = "mpns_channel_url=" + HttpUtility.UrlEncode(e.ChannelUri.ToString());
                if (version != null)
                {
                    postDataStr += "&version=" + version;
                }
                if (langCode != null)
                {
                    postDataStr += "&language_code=" + langCode;
                }
                var req = WebRequest.CreateHttp(uri);
                req.UserAgent = Constants.Net.UserAgent;
                req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                Observable
                .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
                .Invoke()
                .SelectMany
                (
                    stream =>
                    {
                        // POSTデータ
                        var postData = Encoding.UTF8.GetBytes(postDataStr);

                        // 書き込み
                        stream.Write(postData, 0, postData.Length);

                        // ストリームを閉じる
                        stream.Close();

                        // 連結
                        return
                            Observable
                            .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                            .Invoke();
                    }
                )
                .Select<WebResponse, RegisterNotificationChannelResult>
                (
                    res =>
                    {
                        // ストリームを取得
                        Stream stream = res.GetResponseStream();
                        if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                        {
                            stream = new GZipInputStream(stream);
                        }

                        // シリアライズ
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RegisterNotificationChannelResult));
                        var result = (RegisterNotificationChannelResult)serializer.ReadObject(stream);

                        // ストリームを閉じる
                        stream.Close();

                        // 結果
                        return result;
                    }
                )
                .ObserveOnDispatcher()
                .Subscribe
                (
                    s2 =>
                    {
                        callback(s2, null);
                    },
                    e2 =>
                    {
                        notificationChannel.Close();
                        callback(null, e2);
                    }
                );
            };

            //notificationChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

            if (isNewChannel)
            {
                notificationChannel.Open();
                notificationChannel.BindToShellToast();
                notificationChannel.BindToShellTile();
            }
            else
            {
                callback(null, null);
            }
        }

        [DataContract]
        public class UnregisterNotificationChannelResult
        {
            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }
        }

        public void UnregisterNotificationChannel(string uuid, Action<UnregisterNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("UnregisterNotificationChannel");
            HttpNotificationChannel notificationChannel;
            notificationChannel = HttpNotificationChannel.Find(Notification.ChannelName);
            if (notificationChannel != null)
            {
                notificationChannel.ConnectionStatusChanged += (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("NotificationChannel_ConnectionStatusChanged:" + e.ConnectionStatus.ToString());
                };

                notificationChannel.ErrorOccurred += (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("NotificationChannel_ErrorOccurred:" + e.Message.ToString());
                };

                notificationChannel.Close();

                string uri = API.DeviceUnregister;
                System.Diagnostics.Debug.WriteLine(uri);
                string postDataStr = "uuid=" + HttpUtility.UrlEncode(uuid);
                var req = WebRequest.CreateHttp(uri);
                req.UserAgent = Constants.Net.UserAgent;
                req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                Observable
                .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
                .Invoke()
                .SelectMany
                (
                    stream =>
                    {
                        // POSTデータ
                        var postData = Encoding.UTF8.GetBytes(postDataStr);

                        // 書き込み
                        stream.Write(postData, 0, postData.Length);

                        // ストリームを閉じる
                        stream.Close();

                        // 連結
                        return
                            Observable
                            .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                            .Invoke();
                    }
                )
                .Select<WebResponse, UnregisterNotificationChannelResult>
                (
                    res =>
                    {
                        // ストリームを取得
                        Stream stream = res.GetResponseStream();
                        if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                        {
                            stream = new GZipInputStream(stream);
                        }

                        // シリアライズ
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UnregisterNotificationChannelResult));
                        var result = (UnregisterNotificationChannelResult)serializer.ReadObject(stream);

                        // ストリームを閉じる
                        stream.Close();

                        // 結果
                        return result;
                    }
                )
                .ObserveOnDispatcher()
                .Subscribe
                (
                    s2 =>
                    {
                        callback(s2, null);
                    },
                    e2 =>
                    {
                        //callback(null, e2);
                        callback(null, null);
                    }
                );
            }
            else
            {
                callback(null, null);
            }
        }

        [DataContract]
        public class UpdateNotificationChannelResult
        {
            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }
        }

        public void UpdateNotificationChannel(string uuid, string version, string langCode, int[] channelIds, bool resetUnreads, Action<UpdateNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("UpdateNotificationChannel");
            string uri = API.DeviceUpdate;
            System.Diagnostics.Debug.WriteLine(uri);
            string postDataStr;
            postDataStr = "uuid=" + HttpUtility.UrlEncode(uuid);
            if (version != null)
            {
                postDataStr += "&version=" + version;
            }
            if (langCode != null)
            {
                postDataStr += "&language_code=" + langCode;
            }
            if (channelIds != null)
            {
                postDataStr += "&notification_channel_id=" + string.Join(",", channelIds);
            }
            if (resetUnreads)
            {
                postDataStr += "&unread_count=0";
            }
            var req = WebRequest.CreateHttp(uri);
            req.UserAgent = Constants.Net.UserAgent;
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            Observable
            .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
            .Invoke()
            .SelectMany
            (
                stream =>
                {
                    // POSTデータ
                    var postData = Encoding.UTF8.GetBytes(postDataStr);

                    // 書き込み
                    stream.Write(postData, 0, postData.Length);

                    // ストリームを閉じる
                    stream.Close();

                    // 連結
                    return
                        Observable
                        .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                        .Invoke();
                }
            )
            .Select
            (
                res =>
                {
                    // ストリームを取得
                    Stream stream = res.GetResponseStream();
                    if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                    {
                        stream = new GZipInputStream(stream);
                    }

                    // シリアライズ
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UpdateNotificationChannelResult));
                    var result = (UpdateNotificationChannelResult)serializer.ReadObject(stream);

                    // ストリームを閉じる
                    stream.Close();

                    // 結果
                    return result;
                }
            )
            .ObserveOnDispatcher()
            .Subscribe
            (
                s2 =>
                {
                    callback(s2, null);
                },
                e2 =>
                {
                    callback(null, e2);
                }
            );
        }
    }
}