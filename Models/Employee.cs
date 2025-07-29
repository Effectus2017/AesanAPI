namespace Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public int StatusId { get; set; } = 1; // Referencia a OptionSelection con optionKey = 'isActive'
    public int PositionId { get; set; } = 0; // Referencia a OptionSelection con optionKey = 'employeePosition'
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public string AreaCode { get; set; } = "";
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null; // Para convertir empleado en usuario
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}