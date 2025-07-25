namespace TuiNews.Views;

using System.Threading;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TuiNews.Models;
using TuiNews.Services;

public class MainView : Window
{
    private readonly FeedsService feedsService;
    private readonly ListView feedsListView;
    private readonly List<Feed> feeds = [];
    private readonly ListView feedItemsListView;
    private readonly Label titleLabel;
    private readonly Label urlLabel;
    private readonly TextView contentTextView;
    private readonly StatusItem currentStatus;
    private Timer? autoReadTimer;

    public MainView(FeedsService feedsService)
        : base("TUI News")
    {
        this.feedsService = feedsService;

        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();

        feedsListView = new ListView
        {
            X = 0,
            Y = 0,
            Width = 30,
            Height = Dim.Fill() - 1
        };
        feedsListView.SelectedItemChanged += OnFeedSelected;
        Add(feedsListView);

        var verticalLine = new LineView(Orientation.Vertical)
        {
            X = Pos.Right(feedsListView),
            Y = 0,
            Height = Dim.Fill()
        };
        Add(verticalLine);

        feedItemsListView = new ListView
        {
            X = Pos.Right(verticalLine),
            Y = 0,
            Width = Dim.Fill(),
            Height = 10
        };
        feedItemsListView.SelectedItemChanged += OnFeedItemSelected;
        feedItemsListView.OpenSelectedItem += OnFeedItemOpened;
        Add(feedItemsListView);

        var horizontalLine = new LineView(Orientation.Horizontal)
        {
            X = Pos.Right(verticalLine),
            Y = Pos.Bottom(feedItemsListView),
            Width = Dim.Fill()
        };
        Add(horizontalLine);

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

        contentTextView = new TextView
        {
            X = Pos.Right(verticalLine),
            Y = Pos.Bottom(urlLabel),
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true
        };
        Add(contentTextView);

        currentStatus = new StatusItem(Key.Null, "", null);
        var statusBar = new StatusBar(
        [
            new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh Feed", async () => await RefreshFeedAsync()),
            new StatusItem(Key.CtrlMask | Key.T, "~^T~ Refresh All", async () => await RefreshAllFeedsAsync()),
            new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
            currentStatus
        ]);
        Application.Top.Add(statusBar);

        _ = LoadFeedsAsync();
    }

    private async Task LoadFeedsAsync()
    {
        var loadedFeeds = await feedsService.LoadFeedsAsync();
        feeds.Clear();
        feeds.AddRange(loadedFeeds);
        UpdateFeedsListView();
        if (feeds.Any())
        {
            feedsListView.SelectedItem = 0;
            await RefreshAllFeedsAsync();
        }
    }

    private void UpdateFeedsListView()
    {
        var currentFeedSelection = feedsListView.SelectedItem;
        feedsListView.SetSource(feeds.Select(GetFeedTitle).ToList());
        feedsListView.SelectedItem = currentFeedSelection;
    }

    private string GetFeedTitle(Feed feed)
    {
        var unreadCount = feed.Items.Count(i => i.IsUnread);
        return unreadCount > 0 ? $"{feed.Title} ({unreadCount})" : feed.Title ?? string.Empty;
    }

    private async void OnFeedSelected(ListViewItemEventArgs args)
    {
        var feed = feeds[args.Item];
        if (!feed.IsLoaded)
        {
            await feedsService.LoadFeedItemsAsync(feed);
            UpdateFeedsListView();
        }
        UpdateFeedItemsListView(feed);

        if (feed.Items.Any())
        {
            OnFeedItemSelected(new ListViewItemEventArgs(0, feed.Items[0]));
        }
    }

    private void UpdateFeedItemsListView(Feed feed)
    {
        var currentItemSelection = feedItemsListView.SelectedItem;
        feedItemsListView.SetSource(feed.Items.Select(GetFeedItemTitle).ToList());
        feedItemsListView.SelectedItem = currentItemSelection;
    }

    private string GetFeedItemTitle(FeedItem item)
    {
        var unreadIndicator = item.IsUnread ? "* " : "  ";
        return $"{unreadIndicator}{item.PublishDate.DateTime.ToShortDateString(),-10} │ {item.Title}";
    }

    private void OnFeedItemSelected(ListViewItemEventArgs args)
    {
        var feed = feeds[feedsListView.SelectedItem];
        var item = feed.Items[args.Item];

        titleLabel.Text = item.Title;
        urlLabel.Text = item.Link;
        contentTextView.Text = item.Summary;

        if (item.IsUnread)
        {
            autoReadTimer?.Dispose();
            autoReadTimer = new Timer(_ =>
            {
                Application.MainLoop.Invoke(() =>
                {
                    item.IsUnread = false;
                    feed.ReadHashes.Add(item.Fingerprint);
                    _ = feedsService.SaveFeedsAsync(feeds);
                    UpdateFeedItemsListView(feed);
                });
            }, null, 3000, Timeout.Infinite);
        }
    }

    private async void OnFeedItemOpened(ListViewItemEventArgs args)
    {
        var feed = feeds[feedsListView.SelectedItem];
        var item = feed.Items[args.Item];

        Application.Run(new PreView(item));

        if (item.IsUnread)
        {
            item.IsUnread = false;
            feed.ReadHashes.Add(item.Fingerprint);
            await feedsService.SaveFeedsAsync(feeds);
            UpdateFeedItemsListView(feed);
        }
    }

    private async Task RefreshFeedAsync()
    {
        var feed = feeds[feedsListView.SelectedItem];
        SetStatus($"Refreshing feed: {feed.Title}");
        await feedsService.LoadFeedItemsAsync(feed);
        UpdateFeedsListView();
        UpdateFeedItemsListView(feed);
        SetStatus("");
    }

    private async Task RefreshAllFeedsAsync()
    {
        foreach (var feed in feeds)
        {
            await feedsService.LoadFeedItemsAsync(feed);
        }
        UpdateFeedsListView();
        UpdateFeedItemsListView(feeds[feedsListView.SelectedItem]);
    }

    private void SetStatus(string status)
    {
        currentStatus.Title = status;
        Application.Refresh();
    }
}
