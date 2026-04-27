using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using MFO.UserService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<RefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _db.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _db.Update(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}