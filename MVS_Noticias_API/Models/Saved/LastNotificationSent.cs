namespace MVS_Noticias_API.Models.Saved
{
    public class LastNotificationSent
    {
        public int Id { get; set; }
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string RegisterDate { get; set; } = string.Empty;
    }
}
