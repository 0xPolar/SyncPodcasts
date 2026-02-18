using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

using SyncPodcast.Domain.Interfaces;

namespace SyncPodcast.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly JWTSettings _jwtSettings;


    public TokenService(JWTSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public AuthToken GenerateToken(Guid userId)
    {
        var claims = new[]
        { 
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        return new AuthToken
        (
            accessToken,
            refreshToken,
            expiry,
            refreshTokenExpiry
        );
    }

    public Guid? ValidateToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return ExtractUserID(principal);
        }
        catch
        {
            return null;
        }
    }

    public (Guid, AuthToken)? RefreshToken(string expiredToken)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            var principal = tokenHandler.ValidateToken(expiredToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            Guid? userId = ExtractUserID(principal);
            if (userId == null) return null;

            var newToken = GenerateToken(userId.Value);

            return (userId.Value, newToken);
        }
        catch
        {
            return null;
        }
    }

    public static Guid? ExtractUserID(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }
}
