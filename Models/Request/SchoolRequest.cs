public class SchoolRequest
{
    public string Name { get; set; }
    public int EducationLevelId { get; set; }
    public int OperatingPeriodId { get; set; }
    public string Address { get; set; }
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public int ZipCode { get; set; }
    public int OrganizationTypeId { get; set; }
    public List<int> FacilityIds { get; set; }
    public List<int> MealTypeIds { get; set; }
}