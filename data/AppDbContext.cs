using Microsoft.EntityFrameworkCore;
using MultiApp_API.Models;

namespace MultiApp_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(u => u.DocumentType)
                .HasConversion<string>();

            modelBuilder.Entity<Users>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Users_DocumentType",
                    "[DocumentType] IN ('CC', 'NIT', 'Passport')"
                ));
        }
    }
}