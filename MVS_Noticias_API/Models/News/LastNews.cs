namespace MVS_Noticias_API.Models.News
{
    public class LastNews
    {
        public int Id { get; set; }
        public int IdNews { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string SubSection { get; set; } = string.Empty;
        public int IdSection { get; set; }
        public int IdSubSection { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string PhotoMobile { get; set; } = string.Empty;
        public string PhotoCredits { get; set; } = string.Empty;
        public string PhotoDescription { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int IdAuthor { get; set; }
        public string Creator { get; set; } = string.Empty;
        public int IdCreator { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsVideo { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public bool IsSound { get; set; }
        public string SoundUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string HiddenTags { get; set; } = string.Empty;
        public int NewsQuantity { get; set; }
        public int Number { get; set; }
    }
}
