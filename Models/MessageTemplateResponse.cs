namespace Api.Models;

public class MessageTemplateResponse
{
    public int Id { get; set; }
    public required string TemplateKey { get; set; }
    public required string TitleES { get; set; }          // Título del mensaje en español (lo que ve el usuario)
    public required string TitleEN { get; set; }          // Título del mensaje en inglés (lo que ve el usuario)
    public required string BodyES { get; set; }            // Contenido del mensaje en español (lo que ve el usuario)
    public required string BodyEN { get; set; }            // Contenido del mensaje en inglés (lo que ve el usuario)
    public string? Icon { get; set; }                      // Nombre del icono Material (ej: 'heroicons_outline:check-circle')
    public string? Image { get; set; }                     // URL de la imagen a mostrar con el mensaje
    public string? Link { get; set; }                      // Enlace asociado al mensaje (puede ser ruta Angular o URL externa)
    public bool UseRouter { get; set; }                    // Si es true, el Link usa router de Angular; si es false, es URL externa
    public string? PurposeES { get; set; }                 // Propósito del template en español (solo para administración, identifica qué hace este template)
    public string? PurposeEN { get; set; }                 // Propósito del template en inglés (solo para administración, identifica qué hace este template)
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

