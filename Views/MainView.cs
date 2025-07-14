using Terminal.Gui;
using TuiNews.Services;

namespace TuiNews.Views;

public class MainView : Window
{
  public MainView(FeedsService feedsService) : base("TUI News")
  {
    X = 0;
    Y = 0;
    Width = Dim.Fill();
    Height = Dim.Fill();

    var feeds = feedsService.LoadFeeds();
    var feedTitles = feeds.Select(f => f.Title).ToArray();
    var feedsListView = new ListView(feedTitles)
    {
      X = 0,
      Y = 0,
      Width = 30,
      Height = Dim.Fill() - 1
    };
    Add(feedsListView);

    var statusBar = new StatusBar([
      new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh Feed", null),
      new StatusItem(Key.CtrlMask | Key.T, "~^T~ Refresh All", null),
      new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
    ]);

    Application.Top.Add(statusBar);
  }
}