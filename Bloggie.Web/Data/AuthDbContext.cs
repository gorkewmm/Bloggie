using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Seed Roles(User , Admin, SuperAdmin)
            var userRoleId = "396d44dd-d5f7-4cd3-b6dd-64211046a3a6";
            var adminRoleId = "8118b37c-9af8-4822-a477-99529799cc57";
            var superAdminRoleId = "712045be-98fc-4781-ab57-560d15807b93";

            var roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "User",
                    ConcurrencyStamp = userRoleId
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin",
                    ConcurrencyStamp=adminRoleId
                },
                new IdentityRole
                {
                    Id = superAdminRoleId,
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin",
                    ConcurrencyStamp=superAdminRoleId
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            //Seed SuperAdminUser
            var superAdminId = "6035bd50-2555-4e7a-b9e2-42eb03781649";
            var superAdminUser = new IdentityUser
            {
                Id = superAdminId,
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
                NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
            };

            var hasher = new PasswordHasher<IdentityUser>();
            superAdminUser.PasswordHash = hasher.HashPassword(superAdminUser, "Superadmin@123");

            builder.Entity<IdentityUser>().HasData(superAdminUser);

            //Add all roles to SuperAdminUser
            var superAdminRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
            base.OnModelCreating(builder);
        }
    }
}
