namespace TuiNews.Models;

public class FeedItem
{
    public string? Title { get; set; }

    public DateTimeOffset PublishDate { get; set; }

    public string? Link { get; set; }

    public string? Summary { get; set; }
}