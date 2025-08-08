namespace Api.Models.DTO
{
    public class DTOMessage
    {
        public int Id { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Time { get; set; }
        public string? Link { get; set; }
        public bool UseRouter { get; set; }
        public bool Read { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}