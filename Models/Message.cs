using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string? Link { get; set; }
        public bool UseRouter { get; set; } = false;
        public bool Read { get; set; } = false;
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}