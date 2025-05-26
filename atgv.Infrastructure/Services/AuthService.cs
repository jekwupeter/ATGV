using Microsoft.Extensions.Logging;
using atgv.Core.Interfaces;
using atgv.Core.Entities;
using Microsoft.EntityFrameworkCore;
using atgv.Core.Utilities;
using atgv.Core.Models.Common;
using Microsoft.AspNetCore.Identity;

namespace atgv.Infrastructure
{
    public class AuthService(ILogger<AccessTokenService> _logger, AppDbContext _db, ICacheService _cacheService,
        IEmailService _emailService, IPasswordHasher<User> _passwordHasher, IHelpers _helpers)
        : IAuthService
    {
        public async Task<ResponseModel> Login(string email, string password)
        {
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (user is null)
            {
                return new ResponseModel
                {
                    Message = "User not found."
                };
            }
            else if (!user.Verified)
            {
                return new ResponseModel
                {
                    Message = "Signup and verify your account."
                };
            }

            var validateResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (validateResult != PasswordVerificationResult.Success)
            {
                return new ResponseModel
                {
                    Message = "Invalid password."
                };
            }

            string token = _helpers.GenerateJwtToken(user.Email);
            return new ResponseModel
            {
                Data = token,
                Message = "Login successful."
            };
        }

        public async Task<bool> SignUp(string email, string password)
        {
            User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            string passwordHash = string.Empty;

            if (user != null && user.Verified)
            {
                _logger.LogWarning("User with email {Email} already exists.", email);
                return false;
            }
            else if (user != null)
            {
                // remove block on rery revalidate endpoint addtion
                passwordHash = _passwordHasher.HashPassword(user, password);
                user.PasswordHash = passwordHash;
            }
            else
            {
                _logger.LogInformation("Creating new user with email {Email}.", email);

                var newUser = new User
                {
                    Email = email
                };

                passwordHash = _passwordHasher.HashPassword(newUser, password);
                newUser.PasswordHash = passwordHash;

                await _db.Users.AddAsync(newUser);
            }

            await _db.SaveChangesAsync();

            string confirmationToken = Guid.NewGuid().ToString();

            // store the confirmation token in cache for 30 minutes
            // cache mechanism can be Redis, MemoryCache, etc.
            // caching may fail, so we should handle that gracefully
            _cacheService.Set(confirmationToken, email, 30);

            string confirmationLink = $"https://localhost:7204/api/auth/verifyNewUser?token={confirmationToken}";
            string emailBody = $"<h3>Welcome!</h3><p>Please click <a href='{confirmationLink}'>here</a> to confirm your email.</p>";

            bool mailSent = await _emailService.SendEmailAsync(email, "Account Verification", emailBody);

            if (!mailSent)
            {
                _logger.LogError("Failed to send verification email to {Email}", email);
                return false;
            }

            return true;
        }

        public async Task<string> VerifyNewUser(string token)
        {
            string? cachedEmail = _cacheService.Get<string>(token);

            if (string.IsNullOrEmpty(cachedEmail))
            {
                _logger.LogWarning("Verification token {Token} is invalid or expired.", token);
                return "Invalid or expired token.";
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email ==cachedEmail);
            if (user == null)
            {
                _logger.LogCritical("User with email {Email} not found.", cachedEmail);
                return "User not found.";
            }

            user.ModifiedAt = DateTime.Now;
            user.Verified = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation("User with email {Email} has been verified successfully.", cachedEmail);
            
            _cacheService.Remove(token);

            return "User verified successfully.";
        }
    }
}