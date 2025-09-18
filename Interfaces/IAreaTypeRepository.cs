using Api.Models;
using Api.Models.DTO;

namespace Api.Interfaces;

public interface IAreaTypeRepository
{
    Task<dynamic> GetAreaTypeById(int id);
    Task<dynamic> GetAllAreaTypes(int take, int skip, string name, bool alls, bool isList);
    Task<bool> InsertAreaType(AreaTypeRequest areaType);
    Task<bool> UpdateAreaType(DTOAreaType areaType);
    Task<bool> DeleteAreaType(int id);

    /// <summary>
    /// Obtiene el tipo de área válido para una ciudad específica.
    /// </summary>
    /// <param name="cityId">El ID de la ciudad.</param>
    /// <returns>El tipo de área válido para la ciudad.</returns>
    Task<dynamic> GetAreaTypeByCity(int cityId);
}