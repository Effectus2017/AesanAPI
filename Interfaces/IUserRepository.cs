using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Interfaces;

public interface IUserRepository
{

    Task<DTOUser> GetUserById(string userId);
    dynamic GetAllUsersFromDb(int take, int skip, string name, string userId);
    Task<DTOUserResponse> GetAllUsersFromDbWithSP(int take, int skip, string name, int? agencyId = null, List<string> roles = null);
    dynamic GetAllRolesFromDb();
    dynamic GetAllProgramsFromDb(int take, int skip, string name, bool alls);

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para la autenticación
    /// ------------------------------------------------------------------------------------------------

    Task<dynamic> Login(LoginRequest model);
    Task<dynamic> RegisterUserAgency(UserAgencyRequest model);
    Task<dynamic> RegisterUser(DTOUser model, string role, int agencyId);
    Task<dynamic> Update(DTOUser model);
    Task<dynamic> Delete(string userId);

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para cambiar la contraseña y actualizar el avatar
    /// ------------------------------------------------------------------------------------------------

    Task<dynamic> ChangePassword(string userId, string currentPassword, string newPassword);
    Task<string> GeneratePasswordResetToken(string email);
    Task<dynamic> UpdateUserAvatar(string userId, string imageUrl);
    Task<dynamic> DisableUser(string userId);
    Task InsertTemporaryPassword(string userId, string temporaryPassword);
    Task<string?> GetTemporaryPassword(string userId);
    Task DeleteTemporaryPassword(string userId);
    Task<bool> ResetPassword(string userId);
    Task<bool> UpdateTemporalPassword(string email, string newPassword, string temporaryPassword);
    Task<bool> ForcePassword(string userId);

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para generar y validar tokens de restablecimiento de contraseña
    /// ------------------------------------------------------------------------------------------------

    Task<bool> GeneratePasswordResetTokenAndSendEmail(string email);
    Task<bool> ValidatePasswordResetToken(string email, string token);
    Task<bool> ResetPasswordWithToken(string email, string token, string newPassword);
}
