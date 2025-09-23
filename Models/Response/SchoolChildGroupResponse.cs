using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para grupos específicos de niños en servicios de alimentación
/// </summary>
public class SchoolChildGroupResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string GroupName { get; set; }
    public int NumberOfChildren { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
