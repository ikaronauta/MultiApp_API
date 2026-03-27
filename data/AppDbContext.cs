// data/AppDbContext.cs

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

        public DbSet<User> Users { get; set; } // plural para DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Nombre de la tabla
                entity.ToTable("Users");

                // Convertir enum a string
                entity.Property(u => u.DocumentType)
                      .HasConversion<string>();

                // Restricción de valores permitidos
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_DocumentType",
                    "[DocumentType] IN ('CC', 'NIT', 'Passport')"
                ));
            });
        }
    }
}