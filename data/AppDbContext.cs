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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

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
                entity.ToTable("Users", t =>
                    {
                        t.HasCheckConstraint(
                            "CK_Users_DocumentType",
                            "[DocumentType] IN ('CC', 'NIT', 'Passport')"
                        );

                        t.HasCheckConstraint(
                            "CK_Users_Status",
                            "[Status] IN ('Activo', 'Inactivo', 'Bloqueado')"
                        );
                    });

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.CreatedBy)
                    .WithMany(u => u.CreatedUsers)
                    .HasForeignKey(u => u.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.EditedBy)
                    .WithMany(u => u.EditedUsers)
                    .HasForeignKey(u => u.EditedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.CreatedCategories)
                    .HasForeignKey(c => c.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.EditedBy)
                    .WithMany(u => u.EditedCategories)
                    .HasForeignKey(c => c.EditedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasOne(p => p.CreatedBy)
                    .WithMany(u => u.CreatedProducts)
                    .HasForeignKey(p => p.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EditedBy)
                    .WithMany(u => u.EditedProducts)
                    .HasForeignKey(p => p.EditedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Stock).HasDefaultValue(0);

                entity.Property(p => p.MinStock).HasDefaultValue(5);

                entity.Property(p => p.IsActive).HasDefaultValue(true);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
             var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is Category || e.Entity is Product);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is User user)
                    {
                        user.CreatedDate = DateTime.UtcNow;
                        user.EditedDate = DateTime.UtcNow;
                    }
                    else if (entry.Entity is Category category)
                    {
                        category.CreatedDate = DateTime.UtcNow;
                        category.EditedDate = DateTime.UtcNow;
                    }
                    else if (entry.Entity is Product product)
                    {
                        product.CreatedDate = DateTime.UtcNow;
                        product.EditedDate = DateTime.UtcNow;
                        product.Stock = 0;
                        product.MinStock = 5;
                        product.IsActive = true;
                    }
                    
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is User user)
                    {
                        user.EditedDate = DateTime.UtcNow;
                    }
                    else if (entry.Entity is Category category)
                    {
                        category.EditedDate = DateTime.UtcNow;
                    }
                    else if (entry.Entity is Product product)
                    {
                        product.EditedDate = DateTime.UtcNow;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}