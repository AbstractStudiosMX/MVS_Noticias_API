namespace MVS_Noticias_API.DTO.Saved
{
    public class SavedPodcastsDto
    {
        public string userEmail {  get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float PublishedDurationSeconds { get; set; }
        public string ImagePublicUrl { get; set; } = string.Empty;
        public string AudioPublicUrl { get; set; } = string.Empty;
    }
}
