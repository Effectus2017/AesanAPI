using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class PermissionRequest
{
    [Required]
    [StringLength(50)]
    public string ValueKey { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? NameEn { get; set; }

    public bool IsActive { get; set; } = true;
}
