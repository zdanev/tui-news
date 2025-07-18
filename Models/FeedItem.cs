namespace TuiNews.Models;

using System.Security.Cryptography;
using System.Text;

public class FeedItem
{
    public string? Title { get; set; }

    public DateTimeOffset PublishDate { get; set; }

    public string? Link { get; set; }

    public string? Summary { get; set; }

    public bool IsUnread { get; set; } = true;

    public string Fingerprint
    {
        get
        {
            using var sha256 = SHA256.Create();
            var data = Encoding.UTF8.GetBytes($"{Title}{PublishDate:O}{Link}");
            var hash = sha256.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
    }
}
