using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<User>().ToTable(nameof(User));
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique, non-clustered index on email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_User_Email");

        // Filtered unique index for active users
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_User_Email_Active");

        // Composite index
        //modelBuilder.Entity<User>()
        //    .HasIndex(user => new { user.FirstName, user.LastName })
        //    .IsUnique()
        //    .HasDatabaseName("IX_User_FirstName_LastName");
    }
}