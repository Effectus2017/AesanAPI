using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para grupos específicos de niños en servicios de alimentación
/// </summary>
public class SchoolChildGroupRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }
    public string GroupName { get; set; }
    public int NumberOfChildren { get; set; }
}
