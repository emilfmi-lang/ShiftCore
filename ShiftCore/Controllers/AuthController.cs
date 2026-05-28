using Microsoft.AspNetCore.Mvc;
using ShiftCore.Dtos.Admin;
using ShiftCore.Models;
using ShiftCore.Services;

namespace ShiftCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var isRealAdmin = await _authService.VerifyAdminAsync(login);
        if(!isRealAdmin.Data)
            return Unauthorized(ResponseModel<string>.Failure("Giriş qadağandır: Yanlış məlumatlar."));
        var token = _authService.GenerateToken(login.UserName);
        var successResponse = ResponseModel<string>.Success(token);
        return Ok(successResponse);
    }
}
