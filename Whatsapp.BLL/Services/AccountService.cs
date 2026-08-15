using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using noon.Application.DTOs;
using Whatsapp.BLL.DTOs;
using Whatsapp.DAL.models;
using Whatsapp.DAL.Services;

namespace Whatsapp.BLL.Services;

public class AccountService
{
    private readonly UserManager<User> _userManager;
    private IConfiguration _configuration;
    private readonly ILogger<AccountService> _logger;
    private readonly RefreshTokenService _refreshTokenService;
    public AccountService(UserManager<User> userManager, 
        IConfiguration configuration,
        ILogger<AccountService> logger,
        RefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _refreshTokenService = refreshTokenService;
    }
    
    public async Task<AuthTokenDto> GenerateAuthTokenAsync(User user)
    {
        var AccessToken =  await GenerateAccessToken(user);
        int.TryParse(_configuration["JWT:AccessTokenDurationInHour"],out var AccessTokenExpirationTime);
        var AccessTokenExpiresAt = DateTime.UtcNow.AddHours(AccessTokenExpirationTime);
        
        var RefreshToken = GenerateRefreshToken();
        var RefreshTokenHashed = HashRefreshToken(RefreshToken);
        int.TryParse(_configuration["JWT:RefreshTokenDurationInDays"],out var RefreshTokenExpirationTime);
        var RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationTime);
        var NewRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = RefreshTokenHashed,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = RefreshTokenExpiresAt
        };
        var result =  await _refreshTokenService.AddAsync(NewRefreshToken);
        
        return new AuthTokenDto
        {
            FirstName = user.First_Name,
            LastName = user.Last_Name,
            Email = user.Email!,
            AccessToken = AccessToken,
            RefreshToken = RefreshToken,
            AccessTokenExpiresAt = AccessTokenExpiresAt,
            RefreshTokenExpiresAt = RefreshTokenExpiresAt
        };
    }
    public async Task<AuthTokenDto> RefreshTokenAsync(string refreshToken)
    {
        if (String.IsNullOrEmpty(refreshToken))
            return null;
        
        var tokenHash = HashRefreshToken(refreshToken);
        var StoredToken = await _refreshTokenService.GetActiveByHashTokenAsync(tokenHash);
        
        if (StoredToken?.User is null || !StoredToken.IsActive)
            return null;
        
        var newAuthToken = await GenerateAuthTokenAsync(StoredToken.User);
        
        await _refreshTokenService.RevokeAsync(StoredToken, newAuthToken.RefreshToken);
        
        return newAuthToken;
        
    }
    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var tokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _refreshTokenService.GetActiveByHashTokenAsync(tokenHash);

        if (storedToken is null)
            return false;

        await _refreshTokenService.RevokeAsync(storedToken);
        return true;
    }
    public async Task<bool> RevokeAllRefreshTokensAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        await _refreshTokenService.RevokeAllForUserAsync(userId);
        return true;
    }
    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
    private static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }
    private  async Task<string> GenerateAccessToken(User user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        var RolesClaims = userRoles.Select(r => new Claim("role", r));

        var Claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.First_Name}  {user.Last_Name}"),
            }
            .Union(userClaims)
            .Union(RolesClaims);
        
        var SymmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
        var Credintials = new SigningCredentials(SymmetricKey, SecurityAlgorithms.HmacSha256);
        int.TryParse(_configuration["JWT:AccessTokenDurationInHour"],out var ExpiresAt);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer:_configuration["JWT:Issuer"],
            audience:_configuration["JWT:Audience"],
            claims: Claims,
            expires: DateTime.UtcNow.AddHours(ExpiresAt),
            signingCredentials: Credintials );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
    
}