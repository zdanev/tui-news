namespace TuiNews.Services;

using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using TuiNews.Models;

public class FeedsService
{
    private readonly string feedsFilePath;

    public FeedsService(string feedsFilePath)
    {
        this.feedsFilePath = feedsFilePath;
    }

    public List<Feed> LoadFeeds()
    {
        if (!File.Exists(feedsFilePath))
        {
            return new List<Feed>();
        }

        try
        {
            var jsonString = File.ReadAllText(feedsFilePath);
            var feeds = JsonSerializer.Deserialize<List<Feed>>(jsonString) ?? new List<Feed>();
            return feeds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading feeds from {feedsFilePath}: {ex.Message}");
            return new List<Feed>();
        }
    }

    public void SaveFeeds(IEnumerable<Feed> feeds)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(feeds, options);
            File.WriteAllText(feedsFilePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving feeds to {feedsFilePath}: {ex.Message}");
        }
    }

    public void LoadFeedItems(Feed feed)
    {
        if (string.IsNullOrEmpty(feed.Url)) return;

        try
        {
            using var reader = XmlReader.Create(feed.Url);
            var syndicationFeed = SyndicationFeed.Load(reader);

            feed.Title ??= syndicationFeed.Title?.Text;

            var newItems = new List<FeedItem>();
            foreach (var item in syndicationFeed.Items)
            {
                var feedItem = new FeedItem
                {
                    Title = item.Title?.Text,
                    PublishDate = item.PublishDate,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Summary = item.Summary?.Text
                };

                if (feed.ReadHashes.Contains(feedItem.Fingerprint))
                {
                    feedItem.IsUnread = false;
                }
                newItems.Add(feedItem);
            }

            feed.Items = newItems.OrderByDescending(i => i.PublishDate).ToList();
            feed.IsLoaded = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading feed {feed.Url}: {ex.Message}");
            feed.Title = $"Error loading feed: {feed.Url}";
        }
    }
}
