namespace Api.Models;

public class DTOUserDB
{
    public string Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FatherLastName { get; set; } = string.Empty;
    public string MotherLastName { get; set; } = string.Empty;
    public string AdministrationTitle { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? AgencyId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}