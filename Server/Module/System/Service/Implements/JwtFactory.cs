using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace System.Services;

public class JwtFactory : IJwtFactory
{
    private readonly JwtIssuerOptions _jwtOptions;
    private readonly IConfiguration _configuration;

    public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions, IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtOptions = jwtOptions.Value;
        ThrowIfInvalidOptions(_jwtOptions);
    }
    public bool IsValidToken(string token, out string userId)
    {
        token = token.Split("Bearer ").Last();
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration.GetValue<string>("SecretKey");
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var jwtAppSettingOptions = _configuration.GetSection(nameof(JwtIssuerOptions));
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            userId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
            return true;
        }
        catch
        {
            userId = string.Empty;
            return false;
        }
    }
    public async Task<JwtToken> GenerateJwtToken(ClaimsIdentity claimsIdentity)
    {
        claimsIdentity.AddClaims(new Claim[]
        {
            //new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
            new(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
        });

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = claimsIdentity,
            NotBefore = _jwtOptions.NotBefore,
            Expires = _jwtOptions.Expiration,
            SigningCredentials = _jwtOptions.SigningCredentials,
        });

        return new JwtToken
        {
            JwtId = token.Id,
            AccessToken = tokenHandler.WriteToken(token),
        };
    }

    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (options.ValidFor <= TimeSpan.Zero)
        {
            throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
        }

        if (options.SigningCredentials == null)
        {
            throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
        }

        if (options.JtiGenerator == null)
        {
            throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }

    /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() -
                             new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
}

public class JwtToken
{
    public string JwtId { get; set; }
    public string AccessToken { get; set; }
}