namespace Api.Models;

/// <summary>
/// Tipo de servicio de alimentación
/// IDs fijos e inmutables (1000-1009)
/// </summary>
public class ServiceType
{
    /// <summary>
    /// ID del tipo de servicio (fijo e inmutable)
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Nombre del servicio en español
    /// </summary>
    public string name { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del servicio en inglés
    /// </summary>
    public string nameEN { get; set; } = string.Empty;

    /// <summary>
    /// Código único e inmutable (ej: BREAKFAST, LUNCH)
    /// </summary>
    public string code { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional del servicio
    /// </summary>
    public string? description { get; set; }

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int displayOrder { get; set; }

    /// <summary>
    /// Indica si el servicio está activo
    /// </summary>
    public bool isActive { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime createdAt { get; set; }

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime? updatedAt { get; set; }
}

/// <summary>
/// Constantes para IDs de tipos de servicio
/// Estos IDs son fijos y nunca cambian
/// </summary>
public static class ServiceTypeIds
{
    public const int Breakfast = 1;
    public const int Lunch = 2;
    public const int SnackAM = 3;
    public const int Dinner = 4;
    public const int SnackPM = 5;
    public const int SnackNight = 6;
    public const int DinnerExtended = 7;
    public const int DinnerAtRisk = 8;
    public const int SnackExtended = 9;
    public const int SnackAtRisk = 10;
}

