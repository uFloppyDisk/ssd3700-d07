using EmailDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSeeding((context, _) =>
            {
                var managerUser = new IdentityUser
                {
                    Id = "manager-user-id",
                    UserName = "manager@home.com",
                    NormalizedUserName = "MANAGER@HOME.COM",
                    Email = "manager@home.com",
                    NormalizedEmail = "MANAGER@HOME.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                // Hash the password
                var passwordHasher = new PasswordHasher<IdentityUser>();
                managerUser.PasswordHash = passwordHasher.HashPassword(managerUser, "P@ssw0rd!");

                var userRole = new IdentityUserRole<string>
                {
                    UserId = "manager-user-id",
                    RoleId = "ad"
                };

                context.Add(managerUser);
                context.Add(userRole);
                context.SaveChanges();
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // 1) Seed Roles
            // =====================================================
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "ad", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "ma", Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Id = "cu", Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Fix Price datatype warn
            modelBuilder.Entity<Product>()
              .Property(obj => obj.Price).HasPrecision(10, 2);
        }
    }
}

