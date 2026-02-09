using System.Linq;

namespace Api.Constants;

/// <summary>
/// Constantes de roles AESAN para portales admin-portal y aesan-portal.
/// </summary>
public static class AesanRoles
{
    public static readonly string[] All = { Administrator, Monitor, SuperAdmin, ProgramCoordinator };

    public const string Administrator = "Administrator";
    public const string Monitor = "Monitor";
    public const string SuperAdmin = "SuperAdmin";
    public const string ProgramCoordinator = "Program-Coordinator";

    public static bool IsAesanRole(string role) => All.Contains(role);
}
