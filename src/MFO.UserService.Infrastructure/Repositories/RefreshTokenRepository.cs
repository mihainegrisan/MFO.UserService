using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using MFO.UserService.Infrastructure.Persistence;

namespace MFO.UserService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _db.AddAsync(refreshToken, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}