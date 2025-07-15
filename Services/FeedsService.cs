using System.Text.Json;
using System.ServiceModel.Syndication;
using System.Xml;
using TuiNews.Models;

namespace TuiNews.Services;

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
            return JsonSerializer.Deserialize<List<Feed>>(jsonString)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading feeds from {feedsFilePath}: {ex.Message}");
            return new List<Feed>();
        }
    }

    public Feed ReadFeed(string feedUrl)
    {
        try
        {
            using var reader = XmlReader.Create(feedUrl);
            var syndicationFeed = SyndicationFeed.Load(reader);

            var feed = new Feed
            {
                Title = syndicationFeed.Title?.Text,
                Url = feedUrl
            };

            foreach (var item in syndicationFeed.Items)
            {
                feed.Items.Add(new FeedItem
                {
                    Title = item.Title?.Text,
                    PublishDate = item.PublishDate,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Summary = item.Summary?.Text
                });
            }

            return feed;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as appropriate for your application
            Console.WriteLine($"Error reading feed {feedUrl}: {ex.Message}");
            return new Feed
            {
                Title = $"Error loading feed: {feedUrl}",
                Url = feedUrl,
                Items = new List<FeedItem>()
            };
        }
    }
}