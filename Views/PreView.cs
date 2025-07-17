using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Terminal.Gui;
using TuiNews.Models;

namespace TuiNews.Views;

public class PreView : Window
{
    private readonly FeedItem item;

    public PreView(FeedItem item)
    {
        this.item = item;
        Title = item.Title;
        X = 0;
        Y = 1;
        Width = Dim.Fill();
        Height = Dim.Fill();

        var textView = new TextView()
        {
            Text = "Loading...",
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true
        };
        Add(textView);

        if (item.Link is not null)
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
        if (string.IsNullOrEmpty(url))
        {
            textView.Text = "No URL provided.";
            return;
        }

        try
        {
            using var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var article = new StringBuilder();
            var nodes = htmlDoc.DocumentNode.SelectNodes("//p");
            if (nodes is not null)
            {
                foreach (var node in nodes)
                {
                    // Clean up the text by removing extra whitespace and newlines
                    var cleanedText = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(cleanedText))
                    {
                        article.AppendLine(cleanedText);
                        article.AppendLine(); // Add a newline for better readability
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

        return base.ProcessKey(keyEvent);
    }

    
}
