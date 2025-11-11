using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para ProgramPeriod
/// </summary>
public class ProgramPeriodRequest
{
    public int? Id { get; set; }
    public int ProgramId { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

