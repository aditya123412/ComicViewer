using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

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
            new ComicCode()
            {

                GetComicView = (HtmlDocument doc, string host) =>
                {
                    //doc.DocumentNode.QuerySelector(".xcomic-icons").Remove();
                    var html = doc.DocumentNode;

                    var imageNode = html.QuerySelector("[property='og:image']");
                    var src = imageNode.GetAttributeValue("content", "");

                    var previousLink = html.QuerySelector("a.prev").GetAttributeValue("href", "");
                    var nextLink = html.QuerySelector("a.next").GetAttributeValue("href", "");
                    var title = html.QuerySelector("[property='og:title']").GetAttributeValue("content", "");

                    var comic = new ComicView();
                    comic.imageSrc = src;
                    comic.nextURL = $"{nextLink}";
                    comic.prevURL = $"{previousLink}";
                    comic.Title = title;
                    return comic;
                },
                Filter = (string uri) =>
                {
                    return (uri.Contains("arcamax.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var title = url.Replace("https://www.arcamax.com/thefunnies/","").Split('/')[0];
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var imageUrl = $"https://{host}/jsnews/images/comics/{title}.png";

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
            new ComicCode()
            {
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var imageUrl = doc.DocumentNode.QuerySelector("[alt='Explosm']").GetAttributeValue("src", "");
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var newComic = new Comic()
                        {
                            Name = "Cyanide and Happiness",
                            HasHiddenComic = false,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = imageUrl
                        };
                    return newComic;
                },
                Filter = (string uri) =>
                {
                    return (uri.Contains("explosm.net", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComicView = (HtmlDocument doc, string host) =>
                {
                    var imageNode = doc.DocumentNode.QuerySelectorAll("link[as='image']").First(x=>x.GetAttributeValue("href","").Contains("files.explosm.net/comics"));
                    var src = imageNode.GetAttributeValue("href", "");
                    var navLinks =  doc.DocumentNode.QuerySelectorAll("a").Select(x=>x.GetAttributeValue("href","")).Where(x=>x.Contains("/comics/")).Distinct().ToList();

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var comic = new ComicView();
                    comic.imageSrc = src;
                    comic.nextURL = $"https://{host}{navLinks[0]}";
                    if (navLinks.Count==3)
                    {
                        comic.prevURL = $"https://{host}{navLinks[1]}";
                    }

                    comic.lastURL = $"https://explosm.net/comics/latest";
                    comic.Title = title;
                    return comic;
                },
            },
            new ComicCode()
            {
                Filter = (string uri) =>
                {
                    return (uri.Contains("invisiblebread.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var newComic = new Comic()
                        {
                            Name = "Invisible bread",
                            HasHiddenComic = true,
                            CurrentComic = url,
                            Type = host,
                            //LogoImageFileName = imageUrl
                        };
                    return newComic;
                },
                GetComicView = (HtmlDocument doc, string url) =>
                {
                    var imageNode = doc.DocumentNode.QuerySelector("#comic-1").QuerySelector("img");
                    var src = imageNode.GetAttributeValue("src", "");

                    var comic = new ComicView();

                    var nextLink =  doc.DocumentNode.QuerySelector("a.navi-next");
                    if (nextLink!=null)
                    {
                        comic.nextURL = nextLink.GetAttributeValue("href","");
                    }

                    var prevLink =  doc.DocumentNode.QuerySelector("a.navi-prev");
                    if (prevLink!=null)
                    {
                        comic.prevURL = prevLink.GetAttributeValue("href","");
                    }

                    var lastLink =  doc.DocumentNode.QuerySelector("a.navi-last");
                    if (lastLink!=null) {         
                        comic.lastURL = lastLink.GetAttributeValue("href","");
                    }
                    var firstLink =  doc.DocumentNode.QuerySelector("a.navi-first");
                    if (firstLink!=null) {
                        comic.firstURL = firstLink.GetAttributeValue("href","");
                    }

                    var randomLink =  doc.DocumentNode.QuerySelector("a.navi-random").GetAttributeValue("href","");

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    comic.imageSrc = src;
                    comic.randomURL = randomLink;

                    comic.Title = title;
                    return comic;
                }
            },
            new ComicCode()
            {
                Filter = (string uri) =>
                {
                    return (uri.Contains("channelate.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var newComic = new Comic()
                        {
                            Name = "Channelate",
                            HasHiddenComic = true,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = "https://www.channelate.com/images/patreon01.gif"
                        };
                    return newComic;
                },
                GetComicView = (HtmlDocument doc, string url) =>
                {
                    var imageNode = doc.DocumentNode.QuerySelector("#comic").QuerySelector("img");
                    var src = imageNode.GetAttributeValue("src", "");

                    var nextLink =  doc.DocumentNode.QuerySelector("a.navi-next").GetAttributeValue("href","");
                    var prevLink =  doc.DocumentNode.QuerySelector("a.navi-prev").GetAttributeValue("href","");
                    var lastLink =  doc.DocumentNode.QuerySelector("a.navi-last").GetAttributeValue("href","");
                    var firstLink =  doc.DocumentNode.QuerySelector("a.navi-first").GetAttributeValue("href","");
                    //var randomLink =  doc.DocumentNode.QuerySelector("a.navi-random").GetAttributeValue("href","");

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var comic = new ComicView();
                    comic.imageSrc = src;
                    comic.nextURL = nextLink;
                    comic.prevURL = prevLink;
                    //comic.randomURL = randomLink;

                    comic.lastURL = lastLink;
                    comic.firstURL = firstLink;
                    comic.Title = title;
                    return comic;
                }
            },
            new ComicCode()
            {
                Filter = (string uri) =>
                {
                    return (uri.Contains("nedroid.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var newComic = new Comic()
                        {
                            Name = "Nedroid",
                            HasHiddenComic = false,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = "https://nedroid.com/header_banner.png"
                        };
                    return newComic;
                },
                GetComicView = (HtmlDocument doc, string url) =>
                {
                    var imageNode = doc.DocumentNode.QuerySelector("img.comic");
                    var src = "https://www.nedroid.com/" + imageNode.GetAttributeValue("src", "").Replace(" ", "");

                    var navLinks = doc.DocumentNode.QuerySelector("div.navbar").QuerySelectorAll("a").Select(x=>x.GetAttributeValue("href", "")).ToArray();
                    var firstLink =  "https://www.nedroid.com/"+navLinks[0];
                    var prevLink =  "https://www.nedroid.com/"+navLinks[1];
                    var randomLink =  "https://www.nedroid.com/"+navLinks[2];
                    var nextLink =  "https://www.nedroid.com/"+navLinks[3];
                    var lastLink =  "https://www.nedroid.com/" + navLinks[4];

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var comic = new ComicView();
                    comic.imageSrc = src;
                    comic.nextURL = nextLink;
                    comic.prevURL = prevLink;
                    comic.randomURL = randomLink;

                    comic.lastURL = lastLink;
                    comic.firstURL = "https://www.nedroid.com/?1";
                    comic.Title = title;
                    return comic;
                }
            },
        };

        public static ComicCode GetComicCode(string url)
        {
            return Codes.FirstOrDefault(x => x.Filter(url));
        }
    }
}
