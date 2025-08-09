namespace Api.Models.Request;

public class StaffRequest
{
    public int? Id { get; set; }
    public string? FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string? FatherLastName { get; set; } = "";
    public string? MotherLastName { get; set; } = "";
    public int StatusId { get; set; } = 1;
    public int PositionId { get; set; } = 0;
    public int StaffTypeId { get; set; } = 1;
    public int? StaffClassificationId { get; set; }
    public DateTime? ContractStartDate { get; set; } // Fecha de inicio de contrato
    public DateTime? ContractEndDate { get; set; } // Fecha de finalización de contrato
    public DateTime? BirthDate { get; set; }
    public string? Email { get; set; } = "";
    public string? PostalAddress { get; set; } = "";
    public int? CityId { get; set; } = 0;
    public int? RegionId { get; set; } = 0;
    public string? AreaCode { get; set; } = "";
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null;
    public bool IsActive { get; set; } = true;
    
    // Campos de revisión (solo para empleados)
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }
}