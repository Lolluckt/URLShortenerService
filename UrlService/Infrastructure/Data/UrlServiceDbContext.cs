using Microsoft.EntityFrameworkCore;
using UrlService.Domain.Entities;

namespace UrlService.Infrastructure.Data
{
    public class UrlServiceDbContext : DbContext
    {
        public UrlServiceDbContext(DbContextOptions<UrlServiceDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Url> Urls { get; set; }
        public DbSet<About> Abouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Url>().ToTable("Urls");
            modelBuilder.Entity<About>().ToTable("Abouts");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Url>()
                .HasIndex(u => u.OriginalUrl)
                .IsUnique();

            modelBuilder.Entity<Url>()
                .HasIndex(u => u.ShortUrl)
                .IsUnique();

            modelBuilder.Entity<Url>()
                .HasOne(u => u.CreatedBy)
                .WithMany(u => u.Urls)
                .HasForeignKey(u => u.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }
    }
}
