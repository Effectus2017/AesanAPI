using Microsoft.AspNetCore.Identity;
using Api.Models;

namespace Api.Interfaces;

public interface IAuthRepository
{
    Task<IdentityResult> Register(RegisterModel model);
    Task<bool> Login(LoginModel model);
    IEnumerable<IdentityUser> GetUsers();
}
