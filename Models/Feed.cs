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

    [JsonIgnore]
    public bool IsLoaded { get; set; } = false;

    [JsonIgnore]
    public List<FeedItem> Items { get; set; } = [];
}
