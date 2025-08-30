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
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@gmail.com",
                PasswordHash = null,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
