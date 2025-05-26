using Microsoft.Extensions.Logging;
using atgv.Core.Interfaces;
using atgv.Core.Entities;
using Microsoft.EntityFrameworkCore;
using atgv.Core.Utilities;
using atgv.Core.Models.Common;

namespace atgv.Infrastructure
{
    public class AccessTokenService(ILogger<AccessTokenService> _logger, AppDbContext _db,
        IHelpers _helpers)
        : IAccessTokenService
    {
        public async Task<ResponseModel> Generate(string email, DateTime expiryDate)
        {
            var token = _helpers.GenerateAccessToken(expiryDate);

            if (!string.IsNullOrEmpty(token.Error))
            {
                _logger.LogError("Failed to generate access token: {Error}", token.Error);
                return new ResponseModel
                {
                    Message = token.Error
                };
            }

            var tokenRecord = new AccessToken
            {
                Token = token.token,
                ExpiryDate = expiryDate,
                UserEmail = email
            };

            await _db.Tokens.AddAsync(tokenRecord);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Access token generated successfully: {Token}", token.token);

            return new ResponseModel
            {
                Data = token.token,
                Message = "Access token generated successfully."
            };
        }

        public async Task<bool> Validate(string email, string token)
        {
           var tokenEntity = await _db.Tokens
                .AsNoTracking()
                .OrderByDescending(t => t.ExpiryDate)
                .FirstOrDefaultAsync(t => t.Token == token && t.UserEmail == email);

            if (tokenEntity == null)
            {
                _logger.LogWarning("Access token {Token} not found.", token);
                return false;
            }
            if (tokenEntity.ExpiryDate < DateTime.Now)
            {
                _logger.LogWarning("Access token {Token} has expired.", token);
                return false;
            }
            _logger.LogInformation("Access token {Token} is valid.", token);
            return true;
        }

    }
}