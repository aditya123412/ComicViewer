using HtmlAgilityPack;
using ComicViewer;

namespace ComicViewer
{
    public class ComicCode
    {
        public Func<HtmlDocument, string, ComicView> GetComicView;
        public Func<HtmlDocument, string, HttpClient, Comic> GetComic;
        public Func<string, bool> Filter;
        public ComicCode() {
            
        }
    }
}
