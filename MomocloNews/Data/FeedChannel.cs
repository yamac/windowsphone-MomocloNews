using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using SimpleMvvmToolkit;

namespace MomocloNews.Data
{
    [DataContract]
    [Table]
    public class FeedChannel : ModelBase<FeedChannel>
    {
        [Column(IsVersion = true)]
#pragma warning disable
        private Binary version;
#pragma warning restore

        private int _Id;
        [DataMember(Name = "id")]
        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false)]
        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
                NotifyPropertyChanged(m => Id);
            }
        }

        private int _FeedGroupId;
        [DataMember(Name = "feed_group_id")]
        [Column]
        public int FeedGroupId
        {
            get { return _FeedGroupId; }
            set
            {
                if (_FeedGroupId == value) return;
                _FeedGroupId = value;
                NotifyPropertyChanged(m => FeedGroupId);
            }
        }

        private string _FeedLink;
        [DataMember(Name = "feed_link")]
        [Column]
        public string FeedLink
        {
            get { return _FeedLink; }
            set
            {
                if (_FeedLink == value) return;
                _FeedLink = value;
                NotifyPropertyChanged(m => FeedLink);
            }
        }

        private string _Link;
        [DataMember(Name = "link")]
        [Column]
        public string Link
        {
            get { return _Link; }
            set
            {
                if (_Link == value) return;
                _Link = value;
                NotifyPropertyChanged(m => Link);
            }
        }

        private string _Title;
        [DataMember(Name = "title")]
        [Column]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                NotifyPropertyChanged(m => Title);
            }
        }

        private string _FeedItemLastTitle;
        [DataMember(Name = "feed_item_last_title")]
        [Column]
        public string FeedItemLastTitle
        {
            get { return _FeedItemLastTitle; }
            set
            {
                if (_FeedItemLastTitle == value) return;
                _FeedItemLastTitle = value;
                NotifyPropertyChanged(m => FeedItemLastTitle);
            }
        }

        private DateTime _FeedItemLastPublishedAt;
        [DataMember(Name = "feed_item_last_published_at")]
        [Column]
        public DateTime FeedItemLastPublishedAt
        {
            get { return _FeedItemLastPublishedAt; }
            set
            {
                if (_FeedItemLastPublishedAt == value) return;
                _FeedItemLastPublishedAt = value;
                NotifyPropertyChanged(m => FeedItemLastPublishedAt);
            }
        }

        private int _ListOrder;
        [DataMember(Name = "list_order")]
        [Column]
        public int ListOrder
        {
            get { return _ListOrder; }
            set
            {
                if (_ListOrder == value) return;
                _ListOrder = value;
                NotifyPropertyChanged(m => ListOrder);
            }
        }

        private int _AccentColor;
        [DataMember(Name = "accent_color")]
        [Column]
        public int AccentColor
        {
            get { return _AccentColor; }
            set
            {
                if (_AccentColor == value) return;
                _AccentColor = value;
                NotifyPropertyChanged(m => AccentColor);
            }
        }

        private string _AuthorName;
        [DataMember(Name = "author_name")]
        [Column]
        public string AuthorName
        {
            get { return _AuthorName; }
            set
            {
                if (_AuthorName == value) return;
                _AuthorName = value;
                NotifyPropertyChanged(m => AuthorName);
            }
        }

        private string _AuthorNickName;
        [DataMember(Name = "author_nickname")]
        [Column]
        public string AuthorNickName
        {
            get { return _AuthorNickName; }
            set
            {
                if (_AuthorNickName == value) return;
                _AuthorNickName = value;
                NotifyPropertyChanged(m => AuthorNickName);
            }
        }

        private string _ProfileLink;
        [DataMember(Name = "profile_link")]
        [Column]
        public string ProfileLink
        {
            get { return _ProfileLink; }
            set
            {
                if (_ProfileLink == value) return;
                _ProfileLink = value;
                NotifyPropertyChanged(m => ProfileLink);
            }
        }

        private string _ProfileImage;
        [DataMember(Name = "profile_image")]
        [Column]
        public string ProfileImage
        {
            get { return _ProfileImage; }
            set
            {
                if (_ProfileImage == value) return;
                _ProfileImage = value;
                NotifyPropertyChanged(m => ProfileImage);
            }
        }

        private string _Image;
        [DataMember(Name = "image")]
        [Column]
        public string Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;
                _Image = value;
                NotifyPropertyChanged(m => Image);
            }
        }

        private int _Status;
        [DataMember(Name = "status")]
        [Column]
        public int Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;
                _Status = value;
                NotifyPropertyChanged(m => Status);
            }
        }

        private DateTime _UpdatedAt;
        [DataMember(Name = "updated_at")]
        [Column]
        public DateTime UpdatedAt
        {
            get { return _UpdatedAt; }
            set
            {
                if (_UpdatedAt == value) return;
                _UpdatedAt = value;
                NotifyPropertyChanged(m => UpdatedAt);
            }
        }
    }
}
