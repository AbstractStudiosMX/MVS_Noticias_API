using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Models.Currency;
using MVS_Noticias_API.Models.Domain;
using MVS_Noticias_API.Models.News;
using MVS_Noticias_API.Models.Programming;
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
        public DbSet<SavedPodcasts> SavedPodcasts { get; set; }
        public DbSet<SavedVideos> SavedVideos { get; set; }
        public DbSet<Programming> Programs { get; set; }
        public DbSet<BroadcastInfo> BroadcastInfo { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<UserNotifications> Notifications { get; set; }
        public DbSet<LastNews> LastNews { get; set; }
        public DbSet<LastNotificationSent> LastNotificationSent { get; set; }

    }
}
