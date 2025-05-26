namespace atgv.Core.Utilities
{
    public interface IHelpers
    {
        string GenerateJwtToken(string email);
        (string token, string Error) GenerateAccessToken(DateTime expiryDate);
    }
}
