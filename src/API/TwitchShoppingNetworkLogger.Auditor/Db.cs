using Microsoft.EntityFrameworkCore;
using TwitchShoppingNetworkLogger.Auditor.Models;
using TwitchShoppingNetworkLogger.Config;

namespace TwitchShoppingNetworkLogger.Auditor
{
    public class Db : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<WhisperMessage> WhisperMessages { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConfig = ConfigManager.Instance.DatabaseConnection;
            optionsBuilder.UseMySQL($"server={dbConfig.Server};uid={dbConfig.User};pwd={dbConfig.Password};database={dbConfig.Schema};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Build indexes
            modelBuilder.Entity<WhisperMessage>().HasIndex(p => p.FromUserId);
            modelBuilder.Entity<WhisperMessage>().HasIndex(p => p.ToUserId);
            modelBuilder.Entity<WhisperMessage>().HasIndex(p => p.SessionId);
            modelBuilder.Entity<Session>().HasIndex(p => p.UserId);
        }
    }
}
