using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleUrlShortener.Models;
using System.Data;

namespace SimpleUrlShortener.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<ShortUrlDescription> ShortUrlDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ShortUrlDescription>().HasKey(x => x.Id);
            builder.Entity<ShortUrlDescription>()
                   .Property(x => x.Id)
                   .HasMaxLength(ShortUrlDescription.IdMaxLength)
                   .IsUnicode(false);
                    
            base.OnModelCreating(builder);
        }
    }
}
