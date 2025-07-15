namespace TuiNews.Views;

using Terminal.Gui;
using Terminal.Gui.Graphs;
using TuiNews.Services;
using TuiNews.Models;

public class MainView : Window
{
  private readonly FeedsService feedsService;
  private readonly ListView feedsListView;
  private readonly List<Feed> feeds;
  private readonly ListView feedItemsListView;
  private readonly Label titleLabel;
  private readonly Label urlLabel;
  private readonly TextView contentTextView;

  public MainView(FeedsService feedsService) : base("TUI News")
  {
    this.feedsService = feedsService;
    feeds = feedsService.LoadFeeds();

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
    feedsListView.SelectedItemChanged += OnFeedSelectedChanged;
    Add(feedsListView);

    var verticalLine = new LineView(Orientation.Vertical)
    {
      X = Pos.Right(feedsListView),
      Y = 0,
      Height = Dim.Fill()
    };
    // Add(verticalLine);

    feedItemsListView = new ListView()
    {
      X = Pos.Right(verticalLine),
      Y = 0,
      Width = Dim.Fill(),
      Height = 10
    };
    Add(feedItemsListView);

    var horizontalLine = new LineView(Orientation.Horizontal)
    {
      X = Pos.Right(feedsListView),
      Y = Pos.Bottom(feedItemsListView),
      Width = Dim.Fill()
    };
    Add(horizontalLine);
    Add(verticalLine); // !!!

    titleLabel = new Label("Title:")
    {
      X = Pos.Right(verticalLine),
      Y = Pos.Bottom(horizontalLine),
      Width = Dim.Fill(),
      Height = 1
    };
    Add(titleLabel);

    urlLabel = new Label("URL:")
    {
      X = Pos.Right(verticalLine),
      Y = Pos.Bottom(titleLabel),
      Width = Dim.Fill(),
      Height = 1
    };
    Add(urlLabel);

    contentTextView = new TextView()
    {
      X = Pos.Right(verticalLine),
      Y = Pos.Bottom(urlLabel),
      Width = Dim.Fill(),
      Height = Dim.Fill(),
      ReadOnly = true,
      WordWrap = true
    };
    Add(contentTextView);

    var statusBar = new StatusBar([
      new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh Feed", null),
      new StatusItem(Key.CtrlMask | Key.T, "~^T~ Refresh All", null),
      new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
    ]);
    Application.Top.Add(statusBar);

    feedItemsListView.SetSource(feeds[feedsListView.SelectedItem].Items.Select(i => i.Title).ToList());
  }

  private void OnFeedSelectedChanged(ListViewItemEventArgs args)
  {
    var feed = feeds[feedsListView.SelectedItem];
    if (!feed.IsLoaded)
    {
      feed = feedsService.ReadFeed(feed.Url!);
      feeds[feedsListView.SelectedItem] = feed;
    }

    feedItemsListView.SetSource(feed.Items.Select(i => i.Title).ToList());
  }
}