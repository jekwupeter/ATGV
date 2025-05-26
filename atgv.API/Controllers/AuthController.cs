using atgv.Core.Interfaces;
using atgv.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace atgv.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService _userService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<bool> Signup([FromBody] UserModel request)
    {
        var response =await _userService.SignUp(request.Email, request.Password);
        return response;
    }
    
    [HttpPost("login")]
    public async Task<ResponseModel> login([FromBody] UserModel request)
    {
        var response =await _userService.Login(request.Email, request.Password);
        return response;
    }
    
    [HttpGet("verifyNewUser")]
    public async Task<string> VerifyNewUser([FromQuery] string token)
    {
        var response =await _userService.VerifyNewUser(token);
        return response;
    }
}
