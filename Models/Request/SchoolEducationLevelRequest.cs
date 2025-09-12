using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para la relación muchos-a-muchos entre School y EducationLevel
/// </summary>
public class SchoolEducationLevelRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }
    public int EducationLevelId { get; set; }
    public bool IsActive { get; set; } = true;
}
