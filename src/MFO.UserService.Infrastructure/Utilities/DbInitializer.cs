using MFO.UserService.Domain.Entities;
using MFO.UserService.Infrastructure.Data;

namespace MFO.UserService.Infrastructure.Utilities;

public class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Users.Any())
        {
            return; // DB has been seeded
        }

        var users = new List<User>
        {
            new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "John0",
                LastName = "Doe0",
                Email = "random0@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "db-seed"
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "John1",
                LastName = "Doe1",
                Email = "random1@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "db-seed"
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "John2",
                LastName = "Doe2",
                Email = "random2@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "db-seed"
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "John3",
                LastName = "Doe3",
                Email = "random3@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "db-seed"
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "John4",
                LastName = "Doe4",
                Email = "random4@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "db-seed"
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
