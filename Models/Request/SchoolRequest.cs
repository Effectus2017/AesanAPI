namespace Api.Models;
using Swashbuckle.AspNetCore.Annotations;

public class SchoolRequest
{
    [SwaggerSchema(Description = "Identificador único de la escuela")]
    public int Id { get; set; }
    [SwaggerSchema(Description = "Nombre del sitio o escuela")]
    public string Name { get; set; }
    [SwaggerSchema(Description = "ID del nivel educativo")]
    public int EducationLevelId { get; set; }
    [SwaggerSchema(Description = "ID del período de funcionamiento")]
    public int OperatingPeriodId { get; set; }
    [SwaggerSchema(Description = "Dirección física de la escuela")]
    public string Address { get; set; }
    [SwaggerSchema(Description = "ID de la ciudad (pueblo)")]
    public int CityId { get; set; }
    [SwaggerSchema(Description = "ID de la región")]
    public int RegionId { get; set; }
    [SwaggerSchema(Description = "Código postal")]
    public int ZipCode { get; set; }
    [SwaggerSchema(Description = "ID del tipo de organización")]
    public int OrganizationTypeId { get; set; }
    [SwaggerSchema(Description = "Lista de IDs de facilidades asociadas")]
    public List<int>? FacilityIds { get; set; }
    [SwaggerSchema(Description = "Lista de IDs de tipos de comida asociados (si aplica)")]
    public List<int>? MealTypeIds { get; set; }
    [SwaggerSchema(Description = "Fecha de inicio de operaciones del sitio")]
    public DateTime? StartDate { get; set; }
    [SwaggerSchema(Description = "Dirección postal del sitio")]
    public string? PostalAddress { get; set; }
    [SwaggerSchema(Description = "Código de área telefónico")]
    public string? AreaCode { get; set; }
    [SwaggerSchema(Description = "Nombre completo del administrador o representante autorizado")]
    public string? AdminFullName { get; set; }
    [SwaggerSchema(Description = "Teléfono principal de contacto")]
    public string? Phone { get; set; }
    [SwaggerSchema(Description = "Extensión telefónica")]
    public string? PhoneExtension { get; set; }
    [SwaggerSchema(Description = "Número de celular de contacto")]
    public string? Mobile { get; set; }
    [SwaggerSchema(Description = "Año base de operación")]
    public int? BaseYear { get; set; }
    [SwaggerSchema(Description = "Año de próxima renovación")]
    public int? NextRenewalYear { get; set; }
    [SwaggerSchema(Description = "ID del tipo de cocina (catálogo)")]
    public int? KitchenTypeId { get; set; }
    [SwaggerSchema(Description = "ID del tipo de grupo (catálogo)")]
    public int? GroupTypeId { get; set; }
    [SwaggerSchema(Description = "ID del tipo de entrega (catálogo)")]
    public int? DeliveryTypeId { get; set; }
    [SwaggerSchema(Description = "ID del tipo de auspiciador (catálogo)")]
    public int? SponsorTypeId { get; set; }
    [SwaggerSchema(Description = "ID del tipo de solicitante (catálogo)")]
    public int? ApplicantTypeId { get; set; }
    [SwaggerSchema(Description = "ID de la política de funcionamiento (catálogo)")]
    public int? OperatingPolicyId { get; set; }
    [SwaggerSchema(Description = "Indica si la escuela está activa")]
    public bool? IsActive { get; set; }
    [SwaggerSchema(Description = "Fecha de creación del registro")]
    public DateTime? CreatedAt { get; set; }
    [SwaggerSchema(Description = "Fecha de última actualización del registro")]
    public DateTime? UpdatedAt { get; set; }
    [SwaggerSchema(Description = "Lista de IDs de escuelas satélite asociadas")]
    public List<int>? SatelliteSchoolIds { get; set; }
}