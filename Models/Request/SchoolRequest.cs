using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con School
/// </summary>
public class SchoolRequest
{
    public int? Id { get; set; }
    public int AgencyId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
