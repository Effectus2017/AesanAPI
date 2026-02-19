namespace Api.Models;

public class DTOUserRole
{
    public string Id { get; set; } = "";
    /// <summary>Clave única del rol (Identity).</summary>
    public string Name { get; set; } = "";
    /// <summary>Nombre a mostrar en español.</summary>
    public string? DisplayName { get; set; }
    /// <summary>Nombre a mostrar en inglés.</summary>
    public string? DisplayNameEN { get; set; }
    public string? Description { get; set; } = "";
    public string? NormalizedName { get; set; } = "";
    public string? ConcurrencyStamp { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    /// <summary>True = rol principal; False = rol secundario temporal.</summary>
    public bool IsPrimary { get; set; } = true;
    /// <summary>Vigencia desde (solo secundarios).</summary>
    public DateTime? ValidFrom { get; set; }
    /// <summary>Vigencia hasta (solo secundarios).</summary>
    public DateTime? ValidTo { get; set; }
    /// <summary>True si el rol secundario está vigente hoy.</summary>
    public bool IsVigent { get; set; }
    /// <summary>Motivo por el cual se asigna el rol secundario (opcional).</summary>
    public string? Comment { get; set; }
}
