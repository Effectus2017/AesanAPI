using Api.Models;

public class DTOSchool
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int ZipCode { get; set; }
    public DTOCity City { get; set; }
    public DTORegion Region { get; set; }
    public DTOEducationLevel EducationLevel { get; set; }
    public DTOOperatingPeriod OperatingPeriod { get; set; }
    public DTOOrganizationType OrganizationType { get; set; }
    public List<DTOFacility> Facilities { get; set; }
    public List<DTOMealType> MealTypes { get; set; }
}