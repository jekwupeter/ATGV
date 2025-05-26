using atgv.Core.Models.Common;

namespace atgv.Core.Interfaces
{
    public interface IAccessTokenService
    {
        Task<ResponseModel> Generate(string email, DateTime expiryDate);
        Task<bool> Validate(string email, string token);
    }
}
