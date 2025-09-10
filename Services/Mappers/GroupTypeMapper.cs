using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con GroupType
/// Contiene todos los métodos de mapeo para objetos de GroupType
/// </summary>
public class GroupTypeMapper
{
    /// <summary>
    /// Mapea un tipo de grupo desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOGroupType para listas</returns>
    public static DTOGroupType MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOGroupType
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el tipo de grupo para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el tipo de grupo para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un tipo de grupo desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOGroupType para listas paginadas</returns>
    public static DTOGroupType MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOGroupType
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
                IsActive = item.IsActive,
                DisplayOrder = item.DisplayOrder
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el tipo de grupo: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el tipo de grupo: {ex.Message}", ex);
        }
    }
}
