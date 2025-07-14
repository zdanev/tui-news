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

        var jsonString = File.ReadAllText(feedsFilePath);
        return JsonSerializer.Deserialize<List<Feed>>(jsonString)!;
    }

    public Feed ReadFeed(string feedUrl)
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
}