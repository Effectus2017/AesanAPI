namespace Api.Models.Request;

using Api.Models;

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
    public string? ZipCode { get; set; } = "";
    public int? AgencyId { get; set; } = null;
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null;
    public bool IsActive { get; set; } = true;

    // Campos agregados para el sign-up
    public string? PhoneNumber { get; set; } = ""; // Número de teléfono del contacto
    public string? ImageURL { get; set; } = ""; // URL de la imagen/avatar del staff
                                                // AdministrationTitle removido - ahora se maneja a través de PositionId

    // Campos de revisión (solo para empleados)
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }

    // Campos específicos para Miembros de la Junta
    public int? TenureDuration { get; set; } // Tiempo de duración del cargo (numérico)
    public int? TenureDurationUnitId { get; set; } // Referencia a OptionSelection con optionKey = 'tenureDurationUnit'
    public int? ReceivesProgramSalaryId { get; set; } // Referencia a OptionSelection con optionKey = 'yesNo' (¿Recibe salario del programa?)

    // Origen del Salario (selección múltiple: modelos OptionSelection con optionKey = 'salaryOrigin')
    public List<DTOOptionSelection>? SalaryOrigins { get; set; }

    // Asignación a escuela (SchoolStaff)
    public int? SchoolId { get; set; } = null;
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// Contratos por clasificación (1 ítem si Administrativo u Operacional; 2 ítems si Ambos).
    /// Si está vacío o null, se usa PositionId/ContractStartDate/ContractEndDate del request para un solo contrato.
    /// </summary>
    public List<StaffClassificationContractItemRequest>? ClassificationContracts { get; set; }
}