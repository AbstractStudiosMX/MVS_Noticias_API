using Microsoft.AspNetCore.Identity;

namespace MVS_Noticias_API.Models.Domain
{
    public class User
    {
        public int Id { get; set; } 
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; } 
        public string? Gender {  get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public bool IsEnabled   { get; set; }
        public DateTime RegisterDate { get; set; }    
    }
}
