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

            // =====================================================
            // 2) Seed User (manager@home.com)
            // =====================================================
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

            modelBuilder.Entity<IdentityUser>().HasData(managerUser);

            // =====================================================
            // 3) Assign User to Admin role
            // =====================================================
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "manager-user-id",
                    RoleId = "ad"
                }
            );
        }
    }
}

