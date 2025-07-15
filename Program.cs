using Terminal.Gui;
using TuiNews.Services;
using TuiNews.Views;

var feedsService = new FeedsService("feeds.json");

// var feed = feedsService.ReadFeed("https://www.macworld.com/feed");
// foreach (var item in feed.Items)
// {
//   Console.WriteLine(item.Title);
//   Console.WriteLine(item.Link);
//   Console.WriteLine(item.Summary);
//   Console.WriteLine("---");
// }

Application.Init();
Application.Top.Add(new MainView(feedsService));
Application.Run();
Application.Shutdown();