using ProfileSearch.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ProfileSearch.Server.Data
{
    public class ProfileSearchContext : DbContext
    {
        // public ProfileSearchContext(DbContextOptions<ProfileSearchContext> options) : base(options) {}

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Search> Searches { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=profileData;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}