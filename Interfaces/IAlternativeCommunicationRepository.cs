using Api.Models;

namespace Api.Interfaces;

public interface IAlternativeCommunicationRepository
{
    /// <summary>
    /// Obtiene una comunicación alternativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la comunicación alternativa a obtener.</param>
    /// <returns>La comunicación alternativa encontrada o null si no se encuentra.</returns>
    Task<dynamic> GetAlternativeCommunicationById(int id);

    /// <summary>
    /// Obtiene todas las comunicaciones alternativas.
    /// </summary>
    /// <param name="take">El número de comunicaciones alternativas a tomar.</param>
    /// <param name="skip">El número de comunicaciones alternativas a saltar.</param>
    /// <param name="name">El nombre de la comunicación alternativa a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las comunicaciones alternativas.</param>
    /// <returns>Una lista de comunicaciones alternativas.</returns>
    Task<dynamic> GetAllAlternativeCommunications(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta una nueva comunicación alternativa.
    /// </summary>
    /// <param name="alternativeCommunication">La comunicación alternativa a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertAlternativeCommunication(DTOAlternativeCommunication alternativeCommunication);

    /// <summary>
    /// Actualiza una comunicación alternativa existente.
    /// </summary>
    /// <param name="alternativeCommunication">La comunicación alternativa a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateAlternativeCommunication(DTOAlternativeCommunication alternativeCommunication);

    /// <summary>
    /// Elimina una comunicación alternativa existente.
    /// </summary>
    /// <param name="id">El ID de la comunicación alternativa a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteAlternativeCommunication(int id);
}