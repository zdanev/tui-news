using Terminal.Gui;
using Terminal.Gui.Graphs;
using TuiNews.Services;

namespace TuiNews.Views;

public class MainView : Window
{
  private readonly FeedsService feedsService;
  private readonly ListView feedsListView;

  public MainView(FeedsService feedsService) : base("TUI News")
  {
    this.feedsService = feedsService;
    var feeds = feedsService.LoadFeeds();

    X = 0;
    Y = 0;
    Width = Dim.Fill();
    Height = Dim.Fill();

    feedsListView = new ListView(feeds.Select(f => f.Title).ToList())
    {
      X = 0,
      Y = 0,
      Width = 30,
      Height = Dim.Fill() - 1
    };
    Add(feedsListView);

    var verticalLine = new LineView(Orientation.Vertical)
    {
      X = Pos.Right(feedsListView),
      Y = 0,
      Height = Dim.Fill()
    };
    Add(verticalLine);



    var statusBar = new StatusBar([
      new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh Feed", null),
      new StatusItem(Key.CtrlMask | Key.T, "~^T~ Refresh All", null),
      new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
    ]);
    Application.Top.Add(statusBar);
  }
}