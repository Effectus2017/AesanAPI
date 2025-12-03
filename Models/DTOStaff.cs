using Api.Models.Response;

namespace Api.Models;

public class DTOStaff
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public int StatusId { get; set; } = 1;
    public string StatusName { get; set; } = "";
    public int PositionId { get; set; } = 0;
    public string PositionName { get; set; } = "";
    public int StaffTypeId { get; set; } = 1;
    public string StaffTypeName { get; set; } = "";
    public string StaffTypeNameEn { get; set; } = "";
    public int? StaffClassificationId { get; set; } = null;
    public string StaffClassificationName { get; set; } = "";
    public string StaffClassificationNameEn { get; set; } = "";
    public DateTime? ContractStartDate { get; set; } // Fecha de inicio de contrato
    public DateTime? ContractEndDate { get; set; } // Fecha de finalización de contrato
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; } = 0;
    public string CityName { get; set; } = "";
    public int RegionId { get; set; } = 0;
    public string RegionName { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public int? AgencyId { get; set; } = null;
    public string AgencyName { get; set; } = "";
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null;
    public string? UserName { get; set; } = null; // Nombre del usuario si existe
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Campos de revisión (solo para empleados)
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }

    // Datos de la relación SiteStaff
    public int? SiteId { get; set; }
    public bool? IsPrimary { get; set; }

    // Relaciones anidadas (para GetById)
    public DTOCity? City { get; set; }
    public DTORegion? Region { get; set; }
    public DTOOptionSelection? Status { get; set; }
    public DTOOptionSelection? Position { get; set; }
    public DTOStaffType? StaffType { get; set; }
    public DTOStaffClassification? StaffClassification { get; set; }
    public SiteListItemResponse? Site { get; set; }
}