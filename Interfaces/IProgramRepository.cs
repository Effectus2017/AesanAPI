using Api.Models;

namespace Api.Interfaces;

public interface IProgramRepository
{
    /// <summary>
    /// Obtiene un programa por su ID
    /// </summary>
    /// <param name="id">El ID del programa</param>
    /// <returns>El programa</returns>
    Task<dynamic> GetProgramById(int id);

    /// <summary>
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="take">El número de programas a obtener</param>
    /// <param name="skip">El número de programas a saltar</param>
    /// <param name="names">Los nombres de los programas a buscar (separados por coma)</param>
    /// <param name="alls">Si se deben obtener todos los programas</param>
    /// <returns>Los programas</returns>
    Task<dynamic> GetAllProgramsFromDb(int take, int skip, string names, bool alls);

    /// <summary>
    /// Inserta un nuevo programa en la base de datos
    /// </summary>
    /// <param name="programRequest">Datos del programa a insertar</param>
    /// <returns>El ID del programa insertado</returns>
    Task<int> InsertProgram(ProgramRequest programRequest);

    /// <summary>
    /// Inserta una nueva inscripción de programa
    /// </summary>
    /// <param name="request">Datos de la inscripción</param>
    /// <returns>El ID de la inscripción insertada</returns>
    Task<int> InsertProgramInscription(ProgramInscriptionRequest request);

    /// <summary>
    /// Obtiene todas las inscripciones de programas
    /// </summary>
    /// <param name="take">El número de inscripciones a obtener</param>
    /// <param name="skip">El número de inscripciones a saltar</param>
    /// <param name="agencyId">El ID de la agencia</param>
    /// <param name="programId">El ID del programa</param>
    /// <returns>Las inscripciones de programas</returns>
    Task<dynamic> GetAllProgramInscriptions(int take, int skip, int? agencyId = null, int? programId = null);

    Task<List<DTOOptionSelection>> GetAllOptionSelections();
}