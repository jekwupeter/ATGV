using atgv.Core.Interfaces;
using atgv.Core.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace atgv.API.Controllers;

[ApiController]
[Authorize]
[Route("api/accesstoken")]
public class AccessTokenController(IAccessTokenService _svc) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<ResponseModel> GetToken([FromBody] DateTime expiryTime)
    {
        string email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        var response = await _svc.Generate(email, expiryTime);
        return response;
    }

    [HttpPost("validate")]
    public async Task<bool> ValidateToken(
        [FromQuery] 
        string token)
    {
        string email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        
        var response = await _svc.Validate(email, token);
        return response;
    }
}