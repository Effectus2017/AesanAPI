namespace Api.Models;

public class DTOUserById
{
    // Datos de Identity User
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsTemporalPasswordActived { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Datos de Staff (datos personales)
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? ImageURL { get; set; } = "";
    public DateTime? BirthDate { get; set; }
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public string AreaCode { get; set; } = "";
    public int StaffTypeId { get; set; }
    public int StatusId { get; set; }
    public int PositionId { get; set; }
    public int AgencyId { get; set; }
    public DateTime? StaffCreatedAt { get; set; }
    public DateTime? StaffUpdatedAt { get; set; }

    // Nombres de las opciones
    public string AdministrationTitle { get; set; } = "";
    public string StaffTypeName { get; set; } = "";
    public string StatusName { get; set; } = "";

    // Datos de la agencia
    public string AgencyName { get; set; } = "";
    public string AgencyCode { get; set; } = "";

    // Roles del usuario (se manejan por separado en el SP)
}
