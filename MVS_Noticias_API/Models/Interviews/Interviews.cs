namespace MVS_Noticias_API.Models.Interviews
{
    public class Interviews
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorPicture { get; set; } = string.Empty;
        public List<NewsInfo> News { get; set; } = new List<NewsInfo>();

        public class NewsInfo
        {
            public string Header { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ImageUrl {  get; set; } = string.Empty; 
        }
    }
}
