namespace Api.Models.Request;

public class MessageRequest
{
    public int? Id { get; set; }
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Link { get; set; }
    public bool UseRouter { get; set; } = false;
    public string? UserId { get; set; }
}