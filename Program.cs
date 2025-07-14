using TuiNews.Services;

Console.WriteLine("TUI News, Copyright (c) zdanev@");

var feedsService = new FeedsService("feeds.json");
var feeds = feedsService.LoadFeeds();

foreach (var feed in feeds)
{
  Console.WriteLine($"\nReading feed: {feed.Title}");
  var loadedFeed = feedsService.ReadFeed(feed.Url ?? "");
  Console.WriteLine($"Feed Title: {loadedFeed.Title}");

  foreach (var item in loadedFeed.Items)
  {
    Console.WriteLine($"  Title: {item.Title}");
    Console.WriteLine($"  Date: {item.PublishDate}");
    Console.WriteLine($"  URL: {item.Link}");
    Console.WriteLine($"  Summary: {item.Summary}\n");
  }
}