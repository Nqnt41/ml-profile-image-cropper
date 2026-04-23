using ProfileSearch.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ProfileSearch.Server.Data
{
    public class ProfileSearchContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Search> Searches { get; set; } = null!;

        public ProfileSearchContext(DbContextOptions<ProfileSearchContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Image>()
                .Property(i => i.Id)
                .UseIdentityColumn(1, 1);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .UseIdentityColumn(1, 1);

            modelBuilder.Entity<Search>()
                .Property(s => s.Id)
                .UseIdentityColumn(1, 1);
        }
    }
}