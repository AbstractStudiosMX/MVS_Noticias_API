namespace MVS_Noticias_API.Models.Podcast
{
    public class ProgramPodcast
    {
        public int Index { get; set; }
        public string Id { get; set; } = string.Empty;
        public string NetworkId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool HasImage { get; set; }
        public string ImagePublicUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
