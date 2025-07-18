namespace TuiNews.Models;

using System.Text.Json.Serialization;


public class Feed
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("read_hashes")]
    public HashSet<string> ReadHashes { get; set; } = [];

    public bool IsLoaded { get; set; } = false;

    public List<FeedItem> Items { get; set; } = [];
}
