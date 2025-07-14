using System.Text.Json.Serialization;

namespace TuiNews.Models;

public class Feed
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    public List<FeedItem> Items { get; set; } = new List<FeedItem>();
}