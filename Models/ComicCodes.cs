using HtmlAgilityPack;

namespace ComicViewer
{
    public static class ComicCodes
    {
        public static IEnumerable<ComicCode> Codes { get; set; } = new List<ComicCode>
        {

            new ComicCode()
            {
                GetComicView = (HtmlDocument doc, string host) =>
                {
                     // var nodes = doc.DocumentNode.SelectNodes("//.item-comic-image");
                    var imageNode = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("item-comic-image")).First().ChildNodes.Where(x => x.Name.Equals("img", StringComparison.InvariantCultureIgnoreCase)).First();
                    var src = imageNode.GetAttributeValue("src", "");

                    var firstLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-backward")).First().GetAttributeValue("href", "");
                    var previousLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-caret-left")).First().GetAttributeValue("href", "");
                    var nextLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-caret-right")).First().GetAttributeValue("href", "");
                    var randomLink = doc.DocumentNode.Descendants(0).Where(n => n.InnerText.Equals("Random")).First().GetAttributeValue("href", "");
                    var lastLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-forward")).First().GetAttributeValue("href", "");
                    var title = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("media-heading")).First().InnerHtml;

                    var comic = new ComicView();
                    comic.imageSrc = src;
                    comic.nextURL = $"https://{host}{nextLink}";
                    comic.prevURL = $"https://{host}{previousLink}";
                    comic.firstURL = $"https://{host}{firstLink}";
                    comic.lastURL = $"https://{host}{lastLink}";
                    comic.randomURL = $"https://{host}{randomLink}";
                    comic.Title = title;
                    return comic;
                },
                Filter = (string uri) =>
                {
                    return (uri.Contains("gocomics", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var title = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("media-heading")).First().InnerHtml;
                    var imageUrl = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("gc-avatar")).First().ChildNodes.First().GetAttributeValue("src", "");
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var newComic = new Comic()
                        {
                            Name = title,
                            HasHiddenComic = false,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = imageUrl
                        };
                    return newComic;
                }
            },
        };

        public static ComicCode GetComicCode(string url)
        {
            return Codes.FirstOrDefault(x => x.Filter(url));
        }
    }
}
