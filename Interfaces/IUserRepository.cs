using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Interfaces;

public interface IUserRepository
{

    DTOUser GetUserById(string userId);
    dynamic GetAllUsersFromDb(int take, int skip, string name, string userId);
    /// <summary>
    /// Obtiene todos los usuarios usando un stored procedure.
    /// </summary>
    /// <param name="take">El número de usuarios a tomar</param>
    /// <param name="skip">El número de usuarios a saltar</param>
    /// <param name="name">El nombre del usuario a buscar</param>
    /// <param name="agencyId">El ID de la agencia (opcional)</param>
    /// <returns>Una lista de usuarios con el conteo total</returns>
    Task<DTOUserResponse> GetAllUsersFromDbWithSP(int take, int skip, string name, int? agencyId = null);
    dynamic GetAllRolesFromDb();
    dynamic GetAllProgramsFromDb(int take, int skip, string name, bool alls);

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para la autenticación
    /// ------------------------------------------------------------------------------------------------

    Task<dynamic> Login(LoginRequest model);
    Task<dynamic> RegisterUserAgency(UserAgencyRequest model);
    Task<dynamic> RegisterUser(DTOUser model, string role);


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

    /// <summary>
    /// Resetea la contraseña de un usuario usando la contraseña temporal
    /// </summary>
    /// <param name="model">Modelo con email, contraseña temporal y nueva contraseña</param>
    /// <returns>El resultado de la operación</returns>
    Task<IActionResult> ResetPassword(ResetPasswordRequest model);


}
