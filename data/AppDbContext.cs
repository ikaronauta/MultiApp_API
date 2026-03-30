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
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Nombre de la tabla
                entity.ToTable("Users");

                // Convertir enum a string
                entity.Property(u => u.DocumentType)
                      .HasConversion<string>();

                entity.Property(u => u.Status)
                      .HasConversion<string>();

                // Restricción de valores permitidos
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_DocumentType",
                    "[DocumentType] IN ('CC', 'NIT', 'Passport')"
                ));

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Status",
                    "[Status] IN ('Activo', 'Inactivo', 'Bloqueado')"
                ));

                entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}