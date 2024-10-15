using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Models.Domain;
using MVS_Noticias_API.Models.Saved;
using MVS_Noticias_API.Models.Settings;

namespace MVS_Noticias_API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CustomSettings> CustomSettings { get; set; }
        public DbSet<NotificationsSettings> NotificationsSettings { get; set; }
        public DbSet<AccessibilitySettings> AccessibilitySettings { get; set; }
        public DbSet<SavedNews> SavedNews { get; set; }
        public DbSet<SavedVideos> SavedVideos { get; set; }
    }
}
