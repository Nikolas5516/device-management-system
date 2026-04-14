using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DeviceManager.API.Data;
using DeviceManager.API.DTOs;
using DeviceManager.API.Models;

namespace DeviceManager.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Check if email already exists
        if (await _context.AppUsers.AnyAsync(a => a.Email.ToLower() == dto.Email.ToLower()))
            return null;

        // Create the User record first
        var user = new User
        {
            Name = dto.Name,
            Role = "User",
            Location = "Not specified"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create the AppUser with hashed password
        var appUser = new AppUser
        {
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            UserId = user.Id
        };
        _context.AppUsers.Add(appUser);
        await _context.SaveChangesAsync();

        // Generate token
        var token = GenerateToken(appUser, user);

        return new AuthResponseDto
        {
            Token = token,
            Email = appUser.Email,
            Name = user.Name,
            UserId = user.Id
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var appUser = await _context.AppUsers
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Email.ToLower() == dto.Email.ToLower());

        if (appUser == null || !BCrypt.Net.BCrypt.Verify(dto.Password, appUser.PasswordHash))
            return null;

        var token = GenerateToken(appUser, appUser.User!);

        return new AuthResponseDto
        {
            Token = token,
            Email = appUser.Email,
            Name = appUser.User!.Name,
            UserId = appUser.User.Id
        };
    }

    private string GenerateToken(AppUser appUser, User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, appUser.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpireMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}