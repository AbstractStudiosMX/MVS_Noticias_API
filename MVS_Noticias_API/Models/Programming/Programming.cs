namespace MVS_Noticias_API.Models.Programming
{
    public class Programming
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BroadcastHour { get; set; } = string.Empty;
        public string BroadcastDay { get; set; } = string.Empty;
        public string UrlImage { get; set; } = string.Empty;
        public string UrlPersonalSite { get; set; } = string.Empty;
    }
}
