using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para SiteProgram
/// </summary>
public class SiteProgramRequest
{
    public int? Id { get; set; }
    public int SiteId { get; set; }
    public int ProgramId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

