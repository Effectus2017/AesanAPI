using Api.Models;

namespace Api.Interfaces;

public interface ISchoolRepository
{
    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    Task<dynamic> GetAllSchools(int take, int skip, string name, bool alls);

    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    Task<dynamic> GetSchoolById(int id);

    /// <summary>
    /// Obtiene las facilidades de una escuela
    /// </summary>
    Task<List<DTOFacility>> GetSchoolFacilities(int schoolId);

    /// <summary>
    /// Obtiene los tipos de comida de una escuela
    /// </summary>
    Task<List<DTOMealType>> GetSchoolMealTypes(int schoolId);

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    Task<int> InsertSchool(SchoolRequest request);

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    Task<bool> UpdateSchool(int id, SchoolRequest request);

    /// <summary>
    /// Elimina una escuela
    /// </summary>
    Task<bool> DeleteSchool(int id);

    Task<List<DTOMealType>> GetAllMealTypes();
    Task<List<DTOOrganizationType>> GetAllOrganizationTypes();
    Task<List<DTOFacility>> GetAllFacilities();
}