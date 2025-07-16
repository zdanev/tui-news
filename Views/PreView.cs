using Terminal.Gui;
using TuiNews.Models;

namespace TuiNews.Views;

public class PreView : Window
{
    public PreView(FeedItem item)
    {
        Title = item.Title;
        X = 0;
        Y = 1;
        Width = Dim.Fill();
        Height = Dim.Fill();

        var textView = new TextView()
        {
            Text = item.Summary,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true
        };
        Add(textView);
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
