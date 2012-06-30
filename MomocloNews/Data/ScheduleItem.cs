using System;
using System.Runtime.Serialization;
using SimpleMvvmToolkit;

namespace MomocloNews.Data
{
    [DataContract]
    public class ScheduleItem : ModelBase<FeedItem>
    {
        [DataMember(Name = "category_name")]
        public string CategoryName { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        public bool IsLocationAvailable
        {
            get { return Location != null && !Location.Equals(string.Empty) ? true : false; }
        }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        public bool IsContentAvailable
        {
            get { return Content != null && !Content.Equals(string.Empty) ? true : false; }
        }

        [DataMember(Name = "start_at")]
        public DateTime StartAt { get; set; }

        [DataMember(Name = "end_at")]
        public DateTime EndAt { get; set; }

        public DateTime[] StartAtAndEndAt
        {
            get { return new DateTime[2] { StartAt, EndAt }; }
        }
    }
}
