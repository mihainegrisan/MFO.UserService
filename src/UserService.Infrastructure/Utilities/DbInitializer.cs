using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Utilities;

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
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@gmail.com",
                PasswordHash = null,
                CreatedAt = DateTime.Now,
                IsActive = true,
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
