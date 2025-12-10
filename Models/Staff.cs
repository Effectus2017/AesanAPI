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
    public string ZipCode { get; set; } = "";
    public int? AgencyId { get; set; } = null; // Referencia a la agencia a la que pertenece el personal
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null; // Para convertir staff en usuario

    // Campos agregados para el sign-up
    public string? PhoneNumber { get; set; } = ""; // Número de teléfono del contacto
    public string? ImageURL { get; set; } = ""; // URL de la imagen/avatar del staff
                                                // AdministrationTitle removido - ahora se maneja a través de PositionId

    // Campos específicos para Miembros de la Junta
    public int? TenureDuration { get; set; } // Tiempo de duración del cargo (numérico)
    public int? TenureDurationUnitId { get; set; } // Referencia a OptionSelection con optionKey = 'tenureDurationUnit'
    public int? ReceivesProgramSalaryId { get; set; } // Referencia a OptionSelection con optionKey = 'yesNo' (¿Recibe salario del programa?)

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}