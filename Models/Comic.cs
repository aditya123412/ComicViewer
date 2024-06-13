namespace ComicViewer
{
    public class Comic
    {
        public string Name { get; set; }
        public string CurrentComic { get; set; }
        public string Type { get; set; }
        public bool HasHiddenComic { get; set; }
        public string LogoImageFileName { get; set; }
        public ComicCode ComicCode { get; set; }

        public Comic()
        {

        }
    }
}
