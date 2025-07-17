using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _db.Users.FindAsync([id], cancellationToken).AsTask();

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        => _db.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        => _db.Users.AnyAsync(user => user.Email == email, cancellationToken);
    
    public Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.AddAsync(user, cancellationToken);
        _db.SaveChangesAsync(cancellationToken);
        return Task.FromResult(user);
    }
}