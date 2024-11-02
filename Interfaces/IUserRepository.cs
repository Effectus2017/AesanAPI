using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Interfaces;

public interface IUserRepository
{

    DTOUser GetUserById(string userId);
    dynamic GetAllUsersFromDb(int take, int skip, string name, string userId);
    dynamic GetAllRolesFromDb();
    dynamic GetAllProgramsFromDb(int take, int skip, string name, bool alls);

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para la autenticación
    /// ------------------------------------------------------------------------------------------------

    Task<dynamic> Login(LoginRequest model);
    Task<dynamic> RegisterUserAgency(UserAgencyRequest model);
    Task<dynamic> RegisterUser(User model, string role);


    /// ------------------------------------------------------------------------------------------------
    /// Métodos para cambiar la contraseña y actualizar el avatar
    /// ------------------------------------------------------------------------------------------------

    Task<dynamic> ChangePassword(string userId, string currentPassword, string newPassword);
    Task<string> GeneratePasswordResetToken(string email);
    Task<dynamic> UpdateUserAvatar(string userId, string imageUrl);
    Task<dynamic> DisableUser(string userId);
    Task InsertTemporaryPassword(string userId, string temporaryPassword);
    Task DeleteTemporaryPassword(string userId);
}
