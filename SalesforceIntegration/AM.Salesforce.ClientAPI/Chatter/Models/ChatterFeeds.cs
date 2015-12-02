using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Chatter.Models
{
    public class ChatterFeeds
    {
        public List<Feeds> favorites { get; set; }
        public List<Feeds> feeds { get; set; }
    }

    public class Feeds
    {
        public string feedItemsUrl { get; set; }
        public string feedType { get; set; }
        public string feedUrl { get; set; }
        public string keyPrefix { get; set; }
        public string label { get; set; }

        public override string ToString()
        {
            return string.Format("Label: {0}, FeedItemsUrl: {1}, FeedUrl: {2}, FeedType: {3}, KeyPrefix: {4}",
                label ?? "null", feedItemsUrl ?? "null", feedUrl ?? "null", feedType ?? "null", keyPrefix ?? "null");
        }
    }
}
