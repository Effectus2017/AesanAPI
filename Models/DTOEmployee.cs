namespace Api.Models;

public class DTOEmployee
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
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; } = 0;
    public string CityName { get; set; } = "";
    public int RegionId { get; set; } = 0;
    public string RegionName { get; set; } = "";
    public string AreaCode { get; set; } = "";
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null;
    public string? UserName { get; set; } = null; // Nombre del usuario si existe
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}