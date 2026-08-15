using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;

namespace Whatsapp.DAL.Services;
public class RefreshTokenService
{
    private readonly ApplicationDbContext _dbContext;
    public RefreshTokenService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(RefreshToken token)
    {
        await _dbContext.RefreshTokens.AddAsync(token);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetActiveByHashTokenAsync(string HashedToken)
    {
        var result =  await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(r => r.TokenHash == HashedToken &&
                                      r.RevokedAt == null &&
                                      r.ExpiresAt < DateTime.UtcNow);
        return result!;
    }

    public async Task<bool> RevokeAsync(RefreshToken token, string? replacedByTokenHash = null)
    {
        token.ExpiresAt = DateTime.UtcNow;
        token.ReplacedByTokenHash = replacedByTokenHash;
        _dbContext.Update(token);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task RevokeAllForUserAsync(string userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}