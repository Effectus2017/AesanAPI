namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta optimizado para la tabla de escuelas
/// Contiene únicamente los datos necesarios para mostrar en la lista de escuelas
/// Optimizado para rendimiento y transferencia de datos mínima
/// </summary>
public class SchoolTableResponse
{
    /// <summary>
    /// Identificador único de la escuela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la escuela
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física de la escuela
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la ciudad donde está ubicada la escuela
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la región donde está ubicada la escuela
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// Indica si esta escuela es un sitio principal
    /// </summary>
    public bool IsMainSchool { get; set; }

    /// <summary>
    /// Nombre de la escuela principal (si esta es una escuela satélite)
    /// </summary>
    public string? MainSchoolName { get; set; }

    /// <summary>
    /// Matrícula General - Número total de estudiantes matriculados en la escuela
    /// </summary>
    public int? GeneralEnrollment { get; set; }

    /// <summary>
    /// Número de Sitio - Contador consecutivo por agencia
    /// </summary>
    public int SiteNumber { get; set; }

    /// <summary>
    /// Código de la Agencia - Para formar el código completo del sitio
    /// </summary>
    public string? AgencyCode { get; set; }

    /// <summary>
    /// Código completo del sitio - Generado en el backend
    /// </summary>
    public string? SiteCode { get; set; }
}
