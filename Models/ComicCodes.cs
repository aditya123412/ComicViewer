using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Java.Net;

namespace ComicViewer
{
    public static class ComicCodes
    {
        public static HtmlDocument GetDocFromUrl(string url)
        {
            using (var client = new HttpClient())
            {
                var str = client.GetStringAsync(url).Result;
                var Html = new HtmlAgilityPack.HtmlDocument();
                Html.LoadHtml(str);
                return Html;
            }
        }
        public static IEnumerable<ComicCode> Codes { get; set; } = new List<ComicCode>
        {

            new ComicCode()
            {
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                     // var nodes = doc.DocumentNode.SelectNodes("//.item-comic-image");
                    var imageNode = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("item-comic-image")).First().ChildNodes.Where(x => x.Name.Equals("img", StringComparison.InvariantCultureIgnoreCase)).First();
                    var src = imageNode.GetAttributeValue("src", "");

                    var firstLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-backward")).First().GetAttributeValue("href", "");
                    var previousLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-caret-left")).First().GetAttributeValue("href", "");
                    var nextLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-caret-right")).First().GetAttributeValue("href", "");
                    var randomLink = doc.DocumentNode.Descendants(0).Where(n => n.InnerText.Equals("Random")).First().GetAttributeValue("href", "");
                    var lastLink = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("fa-forward")).First().GetAttributeValue("href", "");
                    var title = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("media-heading")).First().InnerHtml;

                    var uri = new Uri(comicUrl);
                    var host = uri.Host;

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = string.IsNullOrEmpty(nextLink)?nextLink: $"https://{host}{nextLink}";
                    comic.prevURL = string.IsNullOrEmpty(previousLink)?nextLink:$"https://{host}{previousLink}";
                    comic.firstURL = $"https://{host}{firstLink}";
                    comic.lastURL = $"https://{host}{lastLink}";
                    comic.randomURL = $"https://{host}{randomLink}";
                    comic.Title = title;
                    comic.Name = name;
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

                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    //doc.DocumentNode.QuerySelector(".xcomic-icons").Remove();
                    var html = doc.DocumentNode;

                    var imageNode = html.QuerySelector("[property='og:image']");
                    var src = imageNode.GetAttributeValue("content", "");

                    var prevBtn = html.QuerySelector("a.prev");
                    var nextBtn = html.QuerySelector("a.next");

                    var previousLink = prevBtn!=null?html.QuerySelector("a.prev").GetAttributeValue("href", ""):"";
                    var nextLink = nextBtn!=null? html.QuerySelector("a.next").GetAttributeValue("href", ""):"";
                    var title = html.QuerySelector("[property='og:title']").GetAttributeValue("content", "");

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = $"{nextLink}";
                    comic.prevURL = $"{previousLink}";
                    comic.Title = title;
                    comic.Name = name;
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
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    var imageNode = doc.DocumentNode.QuerySelectorAll("link[as='image']").First(x=>x.GetAttributeValue("href","").Contains("files.explosm.net/comics"));
                    var src = imageNode.GetAttributeValue("href", "");
                    var navLinks =  doc.DocumentNode.QuerySelectorAll("a").Select(x=>x.GetAttributeValue("href","")).Where(x=>x.Contains("/comics/")).Distinct().ToList();

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var uri = new Uri(comicUrl);
                    var host = uri.Host;

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = $"https://{host}{navLinks[0]}";
                    if (navLinks.Count==3)
                    {
                        comic.prevURL = $"https://{host}{navLinks[1]}";
                    }

                    comic.lastURL = $"https://explosm.net/comics/latest";
                    comic.Title = title;
                    comic.Name=name;
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
                            LogoImageFileName = "https://invisiblebread.com/wp-content/uploads/2011/04/favicon.ico"
                        };
                    return newComic;
                },
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
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

                    comic.imageUrl = src;
                    comic.randomURL = randomLink;
                    comic.Name = name;
                    comic.Title = title;

                    try
                    {
                        var extraPannelLink = doc.DocumentNode.QuerySelector("#extrapanelbutton").QuerySelector("a").GetAttributeValue("href","");
                        doc = GetDocFromUrl(extraPannelLink);
                        comic.hiddenImageUrl = doc.DocumentNode.QuerySelector(".extrapanelimage").GetAttributeValue("src","");
                    }
                    catch (Exception)
                    {}
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
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    var imageNode = doc.DocumentNode.QuerySelector("#comic").QuerySelector("img");
                    var src = imageNode.GetAttributeValue("src", "");

                    var nextLink =  doc.DocumentNode.QuerySelector("a.navi-next").GetAttributeValue("href","");
                    var prevLink =  doc.DocumentNode.QuerySelector("a.navi-prev").GetAttributeValue("href","");
                    var lastLink =  doc.DocumentNode.QuerySelector("a.navi-last").GetAttributeValue("href","");
                    var firstLink =  doc.DocumentNode.QuerySelector("a.navi-first").GetAttributeValue("href","");
                    //var randomLink =  doc.DocumentNode.QuerySelector("a.navi-random").GetAttributeValue("href","");

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = nextLink;
                    comic.prevURL = prevLink;
                    //comic.randomURL = randomLink;

                    comic.lastURL = lastLink;
                    comic.firstURL = firstLink;
                    comic.Title = title;
                    comic.Name = name;

                    var bonusDiv = doc.DocumentNode.QuerySelector("#extrapanelbutton");
                    if (bonusDiv!=null)
                    {
                        using (HttpClient client = new HttpClient()) // WebClient class inherits IDisposable
                        {
                            string htmlCode =  client.GetStringAsync(bonusDiv.QuerySelector("a").GetAttributeValue("href","")).Result;
                            HtmlDocument bonusDoc = new HtmlDocument();
                            bonusDoc.LoadHtml(htmlCode);
                            var pic = bonusDoc.DocumentNode.QuerySelector(".extrapanelimage").GetAttributeValue("src","");
                            comic.hiddenImageUrl = $"https:{pic}";
                        }
                    }
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
                            LogoImageFileName = "https://www.nedroid.com/favicon.ico"
                        };
                    return newComic;
                },
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    var imageNode = doc.DocumentNode.QuerySelector("img.comic");
                    var src = "https://www.nedroid.com/" + imageNode.GetAttributeValue("src", "").Replace(" ", "").Replace("http:","https:");

                    var navLinks = doc.DocumentNode.QuerySelector("div.navbar").QuerySelectorAll("a").Select(x=>x.GetAttributeValue("href", "")).ToArray();
                    var firstLink =  "https://www.nedroid.com/"+navLinks[0];
                    var prevLink =  "https://www.nedroid.com/"+navLinks[1];
                    var randomLink =  "https://www.nedroid.com/"+navLinks[2];
                    var nextLink =  "https://www.nedroid.com/"+navLinks[3];
                    var lastLink =  "https://www.nedroid.com/" + navLinks[4];

                    var title = doc.DocumentNode.QuerySelector("title").InnerHtml;

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = nextLink;
                    comic.prevURL = prevLink;
                    comic.randomURL = randomLink;
                    comic.Name = name;
                    comic.lastURL = lastLink;
                    comic.firstURL = "https://www.nedroid.com/?1";
                    comic.Title = title;
                    return comic;
                }
            },
            new ComicCode()
            {

                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    //doc.DocumentNode.QuerySelector(".xcomic-icons").Remove();
                    var html = doc.DocumentNode;

                    var imageNode = html.QuerySelector("#comic-1").QuerySelector("img");
                    var src = imageNode.GetAttributeValue("src", "");

                    var previousLink = html.QuerySelector("a.navi-next").GetAttributeValue("href", "");
                    var nextLink = html.QuerySelector("a.navi-prev").GetAttributeValue("href", "");

                    var firstLink = html.QuerySelector("a.navi-first").GetAttributeValue("href", "");
                    var lastLink = html.QuerySelector("a.navi-last").GetAttributeValue("href", "");

                    var title = html.QuerySelector("title").GetDirectInnerText();

                    var comic = new ComicView();
                    comic.imageUrl = src;
                    comic.nextURL = $"{nextLink}";
                    comic.prevURL = $"{previousLink}";
                    comic.firstURL = firstLink;
                    comic.lastURL= lastLink;
                    comic.Title = title;
                    comic.Name = name;

                    try
                    {
                        var hiddenPageLink = html.QuerySelector("#question").QuerySelector("a").GetAttributeValue("href","");
                        using(var client = new HttpClient())
                        {
                            string htmlCode = client.GetStringAsync(hiddenPageLink).Result;
                            HtmlDocument hdoc = new HtmlDocument();
                            hdoc.LoadHtml(htmlCode);
                            var hiddenImg = hdoc.DocumentNode.QuerySelector("#comic-1").QuerySelector("img");
                            comic.hiddenImageUrl = hiddenImg.GetAttributeValue("src","").Replace("http:","https:");
                        }
                    }
                    catch (Exception)
                    {
                    }

                    return comic;
                },
                Filter = (string uri) =>
                {
                    return (uri.Contains("amazingsuperpowers.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var title = "Amazing Super Powers";
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var imageUrl = $"https://{host}/jsnews/images/comics/{title}.png";

                    var newComic = new Comic()
                        {
                            Name = title,
                            HasHiddenComic = true,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = imageUrl
                        };
                    return newComic;
                }
            },
            new ComicCode()
            {
                Filter = (string uri) =>
                {
                    return (uri.Contains("extrafabulouscomics.com", StringComparison.InvariantCultureIgnoreCase));
                },
                GetComic = (HtmlDocument doc, string url, HttpClient client) =>
                {
                    var title = "Extra Fabulous Comics";
                    var myUri = new Uri(url);
                    string host = myUri.Host;
                    var imageUrl = $"https://us-a.tapas.io/sa/f0/3988e06b-f7c2-456f-8ec6-0ea12a9c0cde_z.png";

                    var newComic = new Comic()
                        {
                            Name = title,
                            HasHiddenComic = false,
                            CurrentComic = url,
                            Type = host,
                            LogoImageFileName = imageUrl
                        };
                    return newComic;
                },
                GetComicView = (string comicUrl, string name) =>
                {
                    var doc = GetDocFromUrl(comicUrl);
                    //doc.DocumentNode.QuerySelector(".xcomic-icons").Remove();
                    var html = doc.DocumentNode;

                    var imageNode = html.QuerySelector(".post-content__body").QuerySelector("img");
                    var src = imageNode.GetAttributeValue("src", "");

                    var index = int.Parse( html.QuerySelector("title").GetDirectInnerText() );
                    var nextIndex = "____"+(index+1).ToString();
                    var prevIndex = "____"+(index-1).ToString();
                    var randomIndex = "____"+(Random.Shared.Next(1,1234)).ToString();



                    var nextLink = "https://www.extrafabulouscomics.com/"+nextIndex.Substring(nextIndex.Length-5);
                    var prevLink = "https://www.extrafabulouscomics.com/"+prevIndex.Substring(prevIndex.Length-5);
                    var firstLink = "https://www.extrafabulouscomics.com/____1";
                    var lastLink = "https://www.extrafabulouscomics.com/_1234";
                    var randomLink = "https://www.extrafabulouscomics.com/"+randomIndex.Substring(randomIndex.Length-5);

                    //var firstLink = html.QuerySelector("a.navi-first").GetAttributeValue("href", "");
                    //var lastLink = html.QuerySelector("a.navi-last").GetAttributeValue("href", "");

                    var title = html.QuerySelector("title").GetDirectInnerText();

                    var comic = new ComicView();
                    comic.imageUrl = src.Split("/v1")[0];
                    comic.Title = title;
                    comic.Name = name;
                    comic.nextURL = nextLink;
                    comic.prevURL = prevLink;
                    comic.firstURL = firstLink;
                    comic.lastURL = lastLink;
                    comic.randomURL = randomLink;
                    return comic;
                },
            }
        };

        public static ComicCode GetComicCode(string url)
        {
            return Codes.FirstOrDefault(x => x.Filter(url));
        }
    }
}
