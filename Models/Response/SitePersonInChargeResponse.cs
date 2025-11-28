using System;

namespace Api.Models.Response;

public class SitePersonInChargeResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? FatherLastName { get; set; }
    public string? MotherLastName { get; set; }
    public string? SitePhone { get; set; }
    public string? Extension { get; set; }
    public string? MobilePhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

