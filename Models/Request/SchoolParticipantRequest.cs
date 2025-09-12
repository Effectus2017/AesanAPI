using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para la relación muchos-a-muchos entre School y ParticipantType
/// </summary>
public class SchoolParticipantRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }

    /// <summary>
    /// ID del tipo de participante
    /// </summary>
    public int ParticipantTypeId { get; set; }

    public bool IsActive { get; set; } = true;
}
