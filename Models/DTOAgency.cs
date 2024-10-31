namespace Api.Models;

public class DTOAgency
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Name { get; set; } = "";
    public int SdrNumber { get; set; } = 0;
    public int UieNumber { get; set; } = 0;
    public int EinNumber { get; set; } = 0;
    public string Address { get; set; } = "";
    public int PostalCode { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    public DTOCity City { get; set; } = new DTOCity();
    public DTORegion Region { get; set; } = new DTORegion();
    public DTOAgencyStatus Status { get; set; } = new DTOAgencyStatus();
    public DTOProgram Program { get; set; } = new DTOProgram();
    public DTOUser User { get; set; } = new DTOUser();
}
