namespace Api.Models;

/// <summary>
/// DTO para la relación muchos-a-muchos entre School y EducationLevel
/// </summary>
public class DTOSchoolEducationLevel
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int EducationLevelId { get; set; }
    public string EducationLevelName { get; set; }
    public string EducationLevelNameEN { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navegación opcional
    public DTOEducationLevel? EducationLevel { get; set; }
}