using atgv.Core.Models.Common;

namespace atgv.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignUp(string email, string password);
        Task<ResponseModel> Login(string email, string password);
        Task<string> VerifyNewUser(string token);

    }
}
