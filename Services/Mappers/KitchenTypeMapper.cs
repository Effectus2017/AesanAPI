using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con KitchenType
/// Contiene todos los métodos de mapeo para objetos de KitchenType
/// </summary>
public class KitchenTypeMapper
{
    /// <summary>
    /// Mapea un tipo de cocina desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOKitchenType para listas</returns>
    public static DTOKitchenType MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOKitchenType
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el tipo de cocina para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el tipo de cocina para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un tipo de cocina desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOKitchenType para listas paginadas</returns>
    public static DTOKitchenType MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOKitchenType
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
                IsActive = item.IsActive,
                DisplayOrder = item.DisplayOrder,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el tipo de cocina: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el tipo de cocina: {ex.Message}", ex);
        }
    }
}
