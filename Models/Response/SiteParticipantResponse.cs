using System;

namespace Api.Models.Response;

public class SiteParticipantResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public DTOOptionSelection ParticipantType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
