using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Interfaces;
using Api.Models;

namespace Api.Data;

public class AuthRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAuthRepository
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;

    public async Task<IdentityResult> Register(RegisterModel model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<bool> Login(LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        return result.Succeeded;
    }

    public IEnumerable<IdentityUser> GetUsers()
    {
        return _userManager.Users;
    }

    public string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here")); // Usa la misma clave que en appsettings.json
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "your_issuer_here", // Usa el mismo emisor que en appsettings.json
            audience: "your_audience_here", // Usa la misma audiencia que en appsettings.json
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
