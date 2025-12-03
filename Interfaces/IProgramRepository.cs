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
    Task<dynamic> GetAllProgramsFromDb(int take, int skip, string names, bool alls, bool isList);

    /// <summary>
    /// Inserta un nuevo programa en la base de datos
    /// </summary>
    /// <param name="programRequest">Datos del programa a insertar</param>
    /// <returns>El ID del programa insertado</returns>
    Task<bool> InsertProgram(ProgramRequest programRequest);

    /// <summary>
    /// Inserta una nueva inscripción de programa
    /// </summary>
    /// <param name="request">Datos de la inscripción</param>
    /// <returns>El ID de la inscripción insertada</returns>
    Task<bool> InsertProgramInscription(ProgramInscriptionRequest request);

    /// <summary>
    /// Obtiene todas las inscripciones de programas
    /// </summary>
    /// <param name="take">El número de inscripciones a obtener</param>
    /// <param name="skip">El número de inscripciones a saltar</param>
    /// <param name="agencyId">El ID de la agencia</param>
    /// <param name="programId">El ID del programa</param>
    /// <returns>Las inscripciones de programas</returns>
    Task<dynamic> GetAllProgramInscriptions(int take, int skip, int? agencyId = null, int? programId = null);

    /// <summary>
    /// Asigna un evaluador a un programa
    /// </summary>
    /// <param name="userId">ID del usuario evaluador</param>
    /// <param name="programId">ID del programa</param>
    /// <param name="assignedBy">ID del usuario que realiza la asignación</param>
    /// <returns>True si la asignación fue exitosa</returns>
    Task<bool> AssignEvaluatorToProgram(string userId, int programId, string assignedBy);

    /// <summary>
    /// Remueve la asignación de un evaluador a un programa
    /// </summary>
    /// <param name="userId">ID del usuario evaluador</param>
    /// <param name="programId">ID del programa</param>
    /// <returns>True si la remoción fue exitosa</returns>
    Task<bool> RemoveEvaluatorFromProgram(string userId, int programId);

    /// <summary>
    /// Obtiene todos los evaluadores asignados a un programa
    /// </summary>
    /// <param name="programId">ID del programa</param>
    /// <returns>Lista de UserIds de los evaluadores</returns>
    Task<List<string>> GetEvaluatorsByProgramId(int programId);

    /// <summary>
    /// Agrega un evaluador a todos los programas activos automáticamente
    /// </summary>
    /// <param name="userId">ID del usuario evaluador</param>
    /// <param name="assignedBy">ID del usuario que realiza la asignación</param>
    /// <param name="evaluatorRoleId">RoleId del rol Evaluador (GUID)</param>
    /// <returns>Número de programas a los que se agregó el evaluador</returns>
    Task<int> AddEvaluatorToAllActivePrograms(string userId, string assignedBy, string evaluatorRoleId);
}