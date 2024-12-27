namespace MVS_Noticias_API.DTO.Settings
{
    public class DataUserDto
    {
        public string email { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
    }
}
