namespace Api.Models;

public class Staff
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public int StatusId { get; set; } = 1; // Referencia a OptionSelection con optionKey = 'isActive'
    public int PositionId { get; set; } = 0; // Referencia a OptionSelection con optionKey = 'staffPosition'
    public int StaffTypeId { get; set; } = 1; // Referencia a StaffType
    public int? StaffClassificationId { get; set; } = null; // Referencia a StaffClassification
    public DateTime? ContractStartDate { get; set; } // Fecha de inicio de contrato
    public DateTime? ContractEndDate { get; set; } // Fecha de finalización de contrato
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public string AreaCode { get; set; } = "";
    public int? AgencyId { get; set; } = null; // Referencia a la agencia a la que pertenece el personal
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null; // Para convertir staff en usuario
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}