using System.Data.Linq;

namespace MomocloNews.Data
{
    public class FeedDataContext : DataContext
    {
        private const string DBConnectionString = "Data Source=isostore:/Feed.sdf";

        public FeedDataContext()
            : base(DBConnectionString)
        {
            if (DatabaseExists() == false)
            {
                CreateDatabase();
            }
            /*
            else
            {
                DeleteDatabase();
                CreateDatabase();
            }
            */
        }

        public Table<FeedGroup> FeedGroups;
        public Table<FeedChannel> FeedChannels;
    }
}
