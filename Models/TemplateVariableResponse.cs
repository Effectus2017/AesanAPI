namespace Api.Models;

/// <summary>
/// Modelo de respuesta para las variables disponibles en los templates
/// </summary>
public class TemplateVariableResponse
{
    /// <summary>
    /// Nombre de la variable (sin llaves)
    /// Ejemplo: "SiteName"
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Formato de visualización de la variable (con llaves)
    /// Ejemplo: "{SiteName}"
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Descripción de la variable en español
    /// </summary>
    public required string DescriptionES { get; set; }

    /// <summary>
    /// Descripción de la variable en inglés
    /// </summary>
    public required string DescriptionEN { get; set; }

    /// <summary>
    /// Categoría a la que pertenece la variable
    /// Ejemplo: "Sitio", "Auspiciador", "Agencia", "Fecha", "Justificación"
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Ejemplo de uso en español
    /// </summary>
    public required string ExampleES { get; set; }

    /// <summary>
    /// Ejemplo de uso en inglés
    /// </summary>
    public required string ExampleEN { get; set; }

    /// <summary>
    /// Tipo de dato de la variable
    /// Ejemplo: "string", "date", "number"
    /// </summary>
    public required string DataType { get; set; }
}

