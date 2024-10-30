namespace MVS_Noticias_API.Models.Statistics
{
    public class VideoStatistics
    {
        public int IdNews { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public int ViewsNumber {  get; set; }
        public string ImageURL { get; set; } = string.Empty;
    }
}
