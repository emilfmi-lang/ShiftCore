using Microsoft.IdentityModel.Tokens;
using ShiftCore.Dtos.Admin;
using ShiftCore.Infrastructure;
using ShiftCore.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShiftCore.Services;

public class AuthService
{
    private readonly JsonStorage _storage;
    private readonly string _filePath;
    private readonly IConfiguration _config;
    public AuthService(JsonStorage storage , IConfiguration config  )
    {
        _storage = storage;
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "admins.json");
        _config = config;
    }
    public string GenerateToken(string username)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")

        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public async Task<ResponseModel<bool>> VerifyAdminAsync(LoginDto login)
    {
        var admins = await _storage.Read<Admin>(_filePath);
        var isVerified = admins.Any(a => a.UserName == login.UserName && a.Password == login.Password);
        return new ResponseModel<bool> { Data = isVerified };
    }   
}
