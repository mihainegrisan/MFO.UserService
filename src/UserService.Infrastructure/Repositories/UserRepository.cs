using Microsoft.EntityFrameworkCore;
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
        => _db.Users
            .FindAsync([id], cancellationToken)
            .AsTask();

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => _db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<List<User>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        => _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        => _db.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    
    public Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.AddAsync(user, cancellationToken);
        _db.SaveChangesAsync(cancellationToken);
        return Task.FromResult(user);
    }
}