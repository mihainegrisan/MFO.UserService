using Microsoft.EntityFrameworkCore;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using MFO.UserService.Infrastructure.Data;

namespace MFO.UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _db.Users
            .FindAsync([id], cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await _db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task<List<User>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        => await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> SetUserActiveStateAsync(User user, bool isActive, CancellationToken cancellationToken)
    {
        user.IsActive = isActive;
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        => await _db.Users.AnyAsync(user => user.Email == email, cancellationToken);
}