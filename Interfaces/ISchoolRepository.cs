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
    Task<dynamic> GetAllSchoolsFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList);

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    Task<bool> InsertSchool(SchoolRequest request);

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    Task<bool> UpdateSchool(DTOSchool request);

    /// <summary>
    /// Elimina una escuela
    /// </summary>
    Task<bool> DeleteSchool(int id);

    /// <summary>
    /// Verifica si existe una escuela principal en la base de datos
    /// </summary>
    /// <returns>True si existe una escuela principal, false en caso contrario</returns>
    Task<bool> HasMainSchool();

    /// <summary>
    /// Actualiza el estado activo/inactivo de una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> UpdateSchoolActiveStatus(int schoolId, bool isActive, string inactiveJustification = null);
}