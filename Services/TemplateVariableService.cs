namespace Api.Services;

/// <summary>
/// Servicio para obtener las variables disponibles en los templates
/// </summary>
public class TemplateVariableService
{
    /// <summary>
    /// Obtiene todas las variables disponibles para usar en templates
    /// </summary>
    /// <returns>Lista de variables disponibles</returns>
    public List<Models.TemplateVariableResponse> GetAllTemplateVariables()
    {
        return new List<Models.TemplateVariableResponse>
        {
            // Variables de Sitio
            new Models.TemplateVariableResponse
            {
                Key = "SiteName",
                DisplayName = "{SiteName}",
                DescriptionES = "Nombre del sitio",
                DescriptionEN = "Site name",
                Category = "Sitio",
                ExampleES = "El sitio {SiteName} ha sido inactivado.",
                ExampleEN = "Site {SiteName} has been inactivated.",
                DataType = "string"
            },
            new Models.TemplateVariableResponse
            {
                Key = "SiteCode",
                DisplayName = "{SiteCode}",
                DescriptionES = "Código del sitio",
                DescriptionEN = "Site code",
                Category = "Sitio",
                ExampleES = "El sitio {SiteName} (Código: {SiteCode}) ha sido inactivado.",
                ExampleEN = "Site {SiteName} (Code: {SiteCode}) has been inactivated.",
                DataType = "string"
            },

            // Variables de Agencia
            new Models.TemplateVariableResponse
            {
                Key = "AgencyName",
                DisplayName = "{AgencyName}",
                DescriptionES = "Nombre de la agencia",
                DescriptionEN = "Agency name",
                Category = "Agencia",
                ExampleES = "El sitio {SiteName} de la agencia {AgencyName} ha sido inactivado.",
                ExampleEN = "Site {SiteName} from agency {AgencyName} has been inactivated.",
                DataType = "string"
            },

            // Variables de Auspiciador
            new Models.TemplateVariableResponse
            {
                Key = "SponsorName",
                DisplayName = "{SponsorName}",
                DescriptionES = "Nombre del auspiciador",
                DescriptionEN = "Sponsor name",
                Category = "Auspiciador",
                ExampleES = "El auspiciador {SponsorName} ha completado su registro.",
                ExampleEN = "Sponsor {SponsorName} has completed their registration.",
                DataType = "string"
            },
            new Models.TemplateVariableResponse
            {
                Key = "SponsorCode",
                DisplayName = "{SponsorCode}",
                DescriptionES = "Código del auspiciador",
                DescriptionEN = "Sponsor code",
                Category = "Auspiciador",
                ExampleES = "El auspiciador {SponsorName} (Código: {SponsorCode}) ha completado su registro.",
                ExampleEN = "Sponsor {SponsorName} (Code: {SponsorCode}) has completed their registration.",
                DataType = "string"
            },

            // Variables de Fechas
            new Models.TemplateVariableResponse
            {
                Key = "CompletionDate",
                DisplayName = "{CompletionDate}",
                DescriptionES = "Fecha de completación",
                DescriptionEN = "Completion date",
                Category = "Fecha",
                ExampleES = "El auspiciador {SponsorName} ha completado su registro el {CompletionDate}.",
                ExampleEN = "Sponsor {SponsorName} has completed their registration on {CompletionDate}.",
                DataType = "date"
            },
            new Models.TemplateVariableResponse
            {
                Key = "InactiveDate",
                DisplayName = "{InactiveDate}",
                DescriptionES = "Fecha de inactivación",
                DescriptionEN = "Inactivation date",
                Category = "Fecha",
                ExampleES = "El sitio {SiteName} ha sido inactivado el {InactiveDate}.",
                ExampleEN = "Site {SiteName} has been inactivated on {InactiveDate}.",
                DataType = "date"
            },

            // Variables de Justificaciones
            new Models.TemplateVariableResponse
            {
                Key = "InactiveJustification",
                DisplayName = "{InactiveJustification}",
                DescriptionES = "Justificación de inactivación",
                DescriptionEN = "Inactivation justification",
                Category = "Justificación",
                ExampleES = "El sitio {SiteName} ha sido inactivado. Justificación: {InactiveJustification}",
                ExampleEN = "Site {SiteName} has been inactivated. Justification: {InactiveJustification}",
                DataType = "string"
            }
        };
    }
}

