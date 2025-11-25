namespace Api.Models.Request;

public class EmailTemplateRequest
{
    public int? Id { get; set; }
    public required string TemplateKey { get; set; }
    public required string SubjectES { get; set; }
    public required string SubjectEN { get; set; }
    public required string BodyES { get; set; }
    public required string BodyEN { get; set; }
    public string? Description { get; set; }
    public string? DescriptionEN { get; set; }
    public bool IsActive { get; set; }
}

