namespace TuiNews.Views;

using System.Diagnostics;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using Terminal.Gui;
using TuiNews.Models;

public class PreView : Window
{
    private readonly FeedItem item;
    public PreView(FeedItem item)
    {
        this.item = item;
        Title = item.Title;
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();

        var textView = new TextView
        {
            Text = "Loading...",
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true
        };
        Add(textView);

        if (item.Link != null)
        {
            LoadArticle(item.Link, textView);
        }
        else
        {
            textView.Text = "No URL provided.";
        }
    }

    private async void LoadArticle(string url, TextView textView)
    {
        try
        {
            using var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var article = new StringBuilder();
            var nodes = htmlDoc.DocumentNode.SelectNodes("//p");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var cleanedText = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(cleanedText))
                    {
                        article.AppendLine(cleanedText);
                        article.AppendLine();
                    }
                }
            }

            textView.Text = article.ToString();
        }
        catch (Exception ex)
        {
            textView.Text = $"Error loading article: {ex.Message}";
        }
    }

    public override bool ProcessKey(KeyEvent keyEvent)
    {
        if (keyEvent.Key == Key.Esc)
        {
            RequestStop();
            return true;
        }

        if (keyEvent.Key == (Key.O | Key.CtrlMask))
        {
            if (!string.IsNullOrEmpty(item.Link))
            {
                Process.Start(new ProcessStartInfo { FileName = item.Link, UseShellExecute = true });
            }
            return true;
        }

        return base.ProcessKey(keyEvent);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
