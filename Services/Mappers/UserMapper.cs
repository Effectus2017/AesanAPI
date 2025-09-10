using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con User
/// Contiene todos los métodos de mapeo para DTOUser
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Mapea un objeto dynamic a DTOUser
    /// </summary>
    /// <param name="user">Objeto dynamic con datos del usuario</param>
    /// <returns>DTOUser mapeado</returns>
    public static DTOUser MapFromResult(dynamic user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El objeto user no puede ser nulo");
            }

            return new DTOUser
            {
                Id = user.Id ?? string.Empty, // Mantener Id original (UserId)
                StaffId = user.StaffId ?? 0, // Agregar StaffId como campo adicional
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                MiddleName = user.MiddleName ?? string.Empty,
                FatherLastName = user.FatherLastName ?? string.Empty,
                MotherLastName = user.MotherLastName ?? string.Empty,
                AdministrationTitle = user.Position ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ImageURL = user.ImageURL ?? string.Empty,
                IsActive = user.IsActive ?? false,
                IsTemporalPasswordActived = user.IsTemporalPasswordActived ?? false,
                EmailConfirmed = user.EmailConfirmed ?? false,
                Role = user.RoleId != null ? new DTOUserRole
                {
                    Id = user.RoleId ?? string.Empty,
                    Name = user.RoleName ?? string.Empty,
                    NormalizedName = user.RoleNormalizedName ?? string.Empty
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el usuario: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el usuario: {ex.Message}", ex);
        }
    }
}
