using System.Text.Json;
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
}