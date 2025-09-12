using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para la relación muchos-a-muchos entre School y ParticipantType
/// </summary>
public class SchoolParticipantResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }

    /// <summary>
    /// Información del tipo de participante
    /// </summary>
    public DTOOptionSelection? ParticipantType { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
