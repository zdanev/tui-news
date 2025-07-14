using Terminal.Gui;
using TuiNews.Services;
using TuiNews.Views;

var feedsService = new FeedsService("feeds.json");

Application.Init();
Application.Top.Add(new MainView(feedsService));
Application.Run();
Application.Shutdown();