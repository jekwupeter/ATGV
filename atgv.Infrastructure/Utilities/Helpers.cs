using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using atgv.Core.Utilities;
using Microsoft.Extensions.Options;

namespace atgv.Infrastructure
{
    public class Helpers(ILogger<Helpers> _logger, IOptions<JwtSettings> _jwtSettings,
        IOptions<AccessTokenSettings> _accessTokenSettings) : IHelpers
    {
        public string GenerateJwtToken(string email)
        {
            var jwtSettings = _jwtSettings.Value;

            _logger.LogInformation("Generating JWT token for user email {email}", email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtSettings.TokenExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public (string token, string Error) GenerateAccessToken(DateTime expiryDate)
        {
            var accessTokenSettings = _accessTokenSettings.Value;

            DateTime today = DateTime.Now.Date;

            DateTime minAllowedDateTime = DateTime.Now;

            DateTime maxAllowedDateTimeInclusive = today.AddDays(accessTokenSettings.MaxExpiryInDays + 1).AddTicks(-1);

            // Now, validate the input expiryDate
            if (expiryDate < minAllowedDateTime || expiryDate > maxAllowedDateTimeInclusive)
            {
                _logger.LogError("Invalid expiry date provided: {ExpiryDate}. Must be after {MinAllowed} and before {MaxAllowed}.", expiryDate, minAllowedDateTime, maxAllowedDateTimeInclusive);
                return (string.Empty, $"Invalid expiry date. It must not exceed {accessTokenSettings.MaxExpiryInDays} days");
            }

            string token = Guid.NewGuid().ToString("N").Substring(0, accessTokenSettings.TokenLength);
            return (token, string.Empty);
        }
    }

}
