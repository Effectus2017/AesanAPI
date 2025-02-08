namespace Api.Models;

public class DTOUser
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public string AdministrationTitle { get; set; } = "";
    public bool EmailConfirmed { get; set; } = false;
    public List<string> Roles { get; set; } = [];
    public string PhoneNumber { get; set; } = "";
    public string Password { get; set; } = "";
    public string ImageURL { get; set; } = "";
    public bool IsActive { get; set; }
}
