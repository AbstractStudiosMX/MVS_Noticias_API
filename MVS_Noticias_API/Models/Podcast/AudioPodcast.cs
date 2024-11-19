namespace MVS_Noticias_API.Models.Podcast
{
    public class AudioPodcast
    {
        public int Index { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float DurationSeconds { get; set; }
        public float PublishedDurationSeconds { get; set; }
        public string ImagePublicUrl { get; set; } = string.Empty;
        public string AudioPublicUrl { get; set; } = string.Empty;
        public string AudioPublicAdFreeUrl { get; set; } = string.Empty;
    }
}
