using Api.Models;

namespace Api.Interfaces;

public interface ISchoolRepository
{
    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    Task<DTOSchool> GetSchoolById(int id);

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    Task<dynamic> GetAllSchoolsFromDB(int take, int skip, string name, int? cityId, int? regionId, bool alls);

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    Task<bool> InsertSchool(SchoolRequest request);

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    Task<bool> UpdateSchool(SchoolRequest request);

    /// <summary>
    /// Elimina una escuela
    /// </summary>
    Task<bool> DeleteSchool(int id);

}