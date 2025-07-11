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

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _db.Users.FindAsync([id], cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return user;
    }
}