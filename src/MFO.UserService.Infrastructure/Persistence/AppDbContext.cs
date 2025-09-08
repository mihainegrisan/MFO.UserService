using Microsoft.EntityFrameworkCore;
using MFO.UserService.Domain.Entities;

namespace MFO.UserService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hide inactive users from all queries 
        // modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

        // Unique, non-clustered index on email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_User_Email");

        // Filtered unique index for active users
        //modelBuilder.Entity<User>()
        //    .HasIndex(u => u.Email)
        //    .IsUnique()
        //    .HasFilter("[IsActive] = 1")
        //    .HasDatabaseName("IX_User_Email_Active");

        // Composite index
        //modelBuilder.Entity<User>()
        //    .HasIndex(user => new { user.FirstName, user.LastName })
        //    .IsUnique()
        //    .HasDatabaseName("IX_User_FirstName_LastName");

        // modelBuilder.Entity<User>().ToTable(nameof(User));
    }
}