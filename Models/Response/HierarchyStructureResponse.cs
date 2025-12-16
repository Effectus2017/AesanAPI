namespace Api.Models.Response;

/// <summary>
/// Respuesta que contiene la estructura jerárquica completa para el árbol de jerarquía de escuelas
/// </summary>
public class HierarchyStructureResponse
{
    public SponsorNode Sponsor { get; set; }
    public YearNode Year { get; set; }
    public List<SchoolNode> Schools { get; set; }
}

/// <summary>
/// Nodo que representa un Auspiciador Administrador (Nivel 1)
/// </summary>
public class SponsorNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}

/// <summary>
/// Nodo que representa un Año (Nivel 2)
/// </summary>
public class YearNode
{
    public int Year { get; set; }
}

/// <summary>
/// Nodo que representa una Escuela (Nivel 3)
/// </summary>
public class SchoolNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SchoolCode { get; set; }
    public int? SchoolNumber { get; set; }
    public List<SiteNode> Sites { get; set; }
}

/// <summary>
/// Nodo que representa un Sitio (Nivel 4)
/// </summary>
public class SiteNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? SiteNumber { get; set; }
    public string SiteCode { get; set; }
}

