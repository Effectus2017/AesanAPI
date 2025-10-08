namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta simplificado para School en listas y componentes de selección
/// Contiene únicamente los datos básicos necesarios para mostrar en elementos de lista
/// 
/// PROPÓSITO: Este modelo está diseñado específicamente para:
/// - Dropdowns y listas desplegables
/// - Componentes de selección (autocomplete, combobox, etc.)
/// - Listas donde solo se necesita mostrar Id y Name
/// - Casos donde el rendimiento es crítico y no se necesitan datos completos
/// 
/// CUÁNDO USAR: 
/// - En lugar de SchoolResponse cuando solo necesites Id y Name
/// - Para dropdowns en formularios
/// - Para listas de selección en la UI
/// - Cuando el objeto School se usa solo como referencia
/// 
/// CUÁNDO NO USAR:
/// - Cuando necesites datos completos de la escuela (usar SchoolResponse)
/// - Para tablas con múltiples columnas (usar SchoolTableResponse)
/// - Para formularios de edición completa
/// </summary>
public class SchoolListItemResponse
{
    /// <summary>
    /// Identificador único de la escuela
    /// Requerido para operaciones de selección y referencia
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la escuela
    /// Campo principal para mostrar en dropdowns y listas
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
