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
}
