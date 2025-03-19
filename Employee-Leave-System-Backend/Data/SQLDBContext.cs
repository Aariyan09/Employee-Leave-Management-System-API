using Microsoft.EntityFrameworkCore;
using Employee_Leave_System_Backend.Utility;
using Employee_Leave_System_Backend.Entities.DbModels;

namespace Employee_Leave_System_Backend.Data
{
    public class SQLDBContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Ensure email uniqueness


            // Seed an Admin User with a hashed password
            modelBuilder.Entity<Users>().HasData(new Users
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@google.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Hashed password
                Role = Enums.RoleType.Admin
            });


        }

    }
}
