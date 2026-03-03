using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Hosting;

namespace Api;

public static class Utilities
{
    public static dynamic GetErrorListFromModelState(ModelStateDictionary modelState)
    {
        var modelQuery =
            from kvp in modelState
            let field = kvp.Key
            let state = kvp.Value
            where state.Errors.Count > 0
            let val = state.AttemptedValue ?? "[NULL]"
            let errors = string.Join(";", state.Errors.Select(err => err.ErrorMessage))
            select string.Format("{0}: {1}.", field, errors);

        return new { Valid = false, Message = string.Join(", ", modelQuery) };
    }

    public static dynamic GetResponseFromException(Exception ex)
    {
        return new { Valid = false, ex.Message };
    }

    public static string GenerateTemporaryPassword()
    {
        // Implementar lógica para generar una contraseña temporal segura
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    /// <summary>
    /// Obtiene la URL base de la aplicación según el entorno
    /// </summary>
    /// <param name="request">Solicitud HTTP</param>
    /// <returns>URL base de la aplicación</returns>
    public static string GetUrl(HttpRequest request)
    {
        string scheme = request.Scheme;
        string host = request.Host.Value;
        string fullURL = string.Format($"{scheme}://{host}");

        return fullURL;
    }

    /// <summary>
    /// Obtiene la URL según appSettings
    /// </summary>
    /// <param name="appSettings">Configuración de la aplicación</param>
    /// <param name="environment">Entorno de la aplicación</param>
    /// <returns>URL base de la aplicación</returns>
    public static string GetUrl(ApplicationSettings appSettings, IWebHostEnvironment? environment = null)
    {
        string url;

        // Si no se proporciona el entorno, usar directivas de compilación como fallback
        if (environment == null)
        {
#if DEBUG || LOCAL
            url = appSettings.LocalURL;
#elif STAGING
            url = appSettings.StagingURL;
#elif RELEASE
            url = appSettings.ProduccionURL;
#else
            url = appSettings.LocalURL;
#endif
        }
        else
        {
            // Usar el entorno actual
            url = environment.EnvironmentName.ToLower() switch
            {
                "development" => appSettings.LocalURL,
                "staging" => appSettings.StagingURL,
                "production" => appSettings.ProduccionURL,
                _ => appSettings.LocalURL
            };
        }

        // Asegurar que la URL termina con '/'
        if (!url.EndsWith("/"))
        {
            url += "/";
        }

        // Eliminar la parte 'api' si existe, ya que no se necesita para las URLs públicas
        url = url.Replace("/api/", "/").Replace("//", "/").Replace(":/", "://");

        return url;
    }

    /// <summary>
    /// Obtiene un string de una fila dinámica por clave (p. ej. resultado de Dapper Read&lt;dynamic&gt;).
    /// Devuelve null si la fila es null, la clave no existe o el valor es null.
    /// </summary>
    public static string? GetDynamicRowString(dynamic? row, string key)
    {
        if (row == null) return null;
        if (row is IDictionary<string, object> dict && dict.TryGetValue(key, out var val) && val != null)
            return val.ToString();
        return null;
    }

    /// <summary>
    /// Obtiene un string de una fila dinámica por clave, sin distinguir mayúsculas/minúsculas.
    /// Útil cuando el proveedor (p. ej. SQL Server/Dapper) devuelve nombres de columna en distinta capitalización.
    /// </summary>
    public static string? GetDynamicRowStringIgnoreCase(dynamic? row, string key)
    {
        if (row == null) return null;
        if (row is not IDictionary<string, object> dict) return null;
        foreach (var kvp in dict)
        {
            if (kvp.Key != null && string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                return kvp.Value.ToString();
        }
        return null;
    }

    public static string RemoveSpecialCharacters(string str)
    {
        string _modifier = Regex.Replace(str, "[^a-zA-Z0-9_.]+", "_", RegexOptions.Compiled);
        _modifier = Regex.Replace(_modifier, ".jpg", "", RegexOptions.Compiled);
        _modifier = Regex.Replace(_modifier, ".jpeg", "", RegexOptions.Compiled);
        _modifier = Regex.Replace(_modifier, ".bmp", "", RegexOptions.Compiled);
        _modifier = Regex.Replace(_modifier, ".gif", "", RegexOptions.Compiled);
        return _modifier;
    }

    public static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Genera un código identificador único para una agencia
    /// </summary>
    /// <param name="agencyName">Nombre de la agencia</param>
    /// <param name="programIds">IDs de los programas</param>
    /// <param name="existingCodes">Códigos existentes para validar unicidad</param>
    /// <returns>Código identificador único</returns>
    public static string GenerateAgencyCode(string agencyName, List<int> programIds, List<string> existingCodes)
    {
        // Generar iniciales de la agencia (3 letras)
        string initials = GenerateInitials(agencyName);

        // Asegurar que las iniciales sean únicas
        string uniqueInitials = EnsureUniqueInitials(initials, existingCodes);

        // Generar número aleatorio de 3 dígitos
        string randomNum = new Random().Next(100, 999).ToString();

        // Ordenar y formatear los IDs de programa (P1, P2, etc.)
        var programParts = programIds.OrderBy(p => p).Select(p => $"P{p}").ToList();
        string programsStr = string.Join("-", programParts);

        // Obtener el año actual
        string year = DateTime.Now.Year.ToString();

        // Obtener el siguiente número de secuencia
        string sequence = GetNextSequenceNumber(existingCodes, year);

        // Construir el código final
        return $"{uniqueInitials}{randomNum}-{programsStr}-{year}-{sequence}";
    }

    /// <summary>
    /// Genera un código identificador único para un sitio basado en SchoolCode
    /// </summary>
    /// <param name="schoolCode">Código de la escuela (ejemplo: 001)</param>
    /// <param name="siteNumber">Número del sitio</param>
    /// <returns>Código identificador único del sitio en formato SchoolCode-SiteNumber</returns>
    public static string GenerateSiteCodeFromSchool(string schoolCode, int siteNumber)
    {
        return $"{schoolCode}-{siteNumber}";
    }

    /// <summary>
    /// Genera un código identificador único para un sitio basado en número de secuencia de agencia
    /// </summary>
    /// <param name="agencySequenceNumber">Número de secuencia de la agencia (string)</param>
    /// <param name="existingCodes">Códigos existentes para validar unicidad</param>
    /// <returns>Código identificador único del sitio en formato {agencySequenceNumber}-{secuencia}</returns>
    public static string GenerateSiteCode(string agencySequenceNumber, List<string> existingCodes)
    {
        // Generar código con formato: {agencySequenceNumber}-{secuencia} (sin prefijo "S" ni año)
        string sequence = GetNextSiteSequenceNumber(existingCodes, agencySequenceNumber);
        return $"{agencySequenceNumber}-{sequence}";
    }

    /// <summary>
    /// Genera un código identificador simple para una agencia con formato T-{año}-{secuencia}
    /// </summary>
    /// <param name="existingCodes">Códigos existentes para validar unicidad</param>
    /// <returns>Código identificador único en formato T-{año}-{secuencia}</returns>
    public static string GenerateSimpleAgencyCode(List<string> existingCodes)
    {
        // Obtener el año actual
        string year = DateTime.Now.Year.ToString();

        // Obtener el siguiente número de secuencia para el año actual
        string sequence = GetNextSimpleSequenceNumber(existingCodes, year);

        // Construir el código final: T-{año}-{secuencia}
        return $"T-{year}-{sequence}";
    }

    /// <summary>
    /// Obtiene el siguiente número de secuencia para un código simple en formato T-{año}-{secuencia}
    /// </summary>
    /// <param name="existingCodes"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private static string GetNextSimpleSequenceNumber(List<string> existingCodes, string year)
    {
        // Filtrar códigos del año actual que siguen el formato T-{año}-{secuencia}
        var yearCodes = existingCodes.Where(c => c.StartsWith($"T-{year}-"))
                                   .Select(c => int.Parse(c.Split('-').Last()))
                                   .DefaultIfEmpty(0)
                                   .Max();

        // Incrementar el número de secuencia y formatear con ceros a la izquierda
        return (yearCodes + 1).ToString("D3");
    }

    /// <summary>
    /// Genera un código único basado en un número aleatorio
    /// </summary>
    /// <returns>Código único en formato T-{numero}</returns>
    public static string GenerateRandomTCode()
    {
        // Generar un número aleatorio de 8 dígitos
        var random = new Random();
        int randomNumber = random.Next(10000000, 99999999);

        // Formatear como: T-{numero}
        return $"T-{randomNumber}";
    }

    private static string GenerateInitials(string name)
    {
        // Remover caracteres especiales y diacríticos
        string cleanName = RemoveDiacritics(name).ToUpper();

        // Dividir en palabras
        var words = cleanName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length >= 3)
        {
            // Tomar la primera letra de las primeras tres palabras
            return $"{words[0][0]}{words[1][0]}{words[2][0]}";
        }
        else if (words.Length == 2)
        {
            // Tomar dos letras de la primera palabra y una de la segunda
            return $"{words[0][0]}{(words[0].Length > 1 ? words[0][1] : 'X')}{words[1][0]}";
        }
        else
        {
            // Tomar las primeras tres letras de la única palabra
            return words[0].Length >= 3 ? words[0][..3] : words[0].PadRight(3, 'X');
        }
    }

    private static string EnsureUniqueInitials(string initials, List<string> existingCodes)
    {
        string result = initials;
        int counter = 1;

        // Si las iniciales ya existen, agregar un número al final
        while (existingCodes.Any(code => code.StartsWith(result)))
        {
            result = $"{initials}{counter}";
            counter++;
        }

        return result;
    }

    private static string GetNextSequenceNumber(List<string> existingCodes, string year)
    {
        // Filtrar códigos del año actual
        var yearCodes = existingCodes.Where(c => c.Contains($"-{year}-"))
                                   .Select(c => int.Parse(c.Split('-').Last()))
                                   .DefaultIfEmpty(0)
                                   .Max();

        // Incrementar el número de secuencia y formatear con ceros a la izquierda
        return (yearCodes + 1).ToString("D3");
    }

    private static int GetNextSiteNumber(List<string> existingSiteCodes, string agencySequenceNumber)
    {
        // Filtrar códigos de sitios que pertenecen a la agencia específica
        var agencySiteCodes = existingSiteCodes.Where(c => c.StartsWith($"{agencySequenceNumber}-"))
                                              .Select(c => int.Parse(c.Split('-').Last()))
                                              .DefaultIfEmpty(0)
                                              .Max();

        // Incrementar el número del sitio
        return agencySiteCodes + 1;
    }

    /// <summary>
    /// Obtiene el siguiente número de secuencia para un código de sitio en formato {agencySequenceNumber}-{secuencia}
    /// </summary>
    /// <param name="existingCodes">Lista de códigos existentes</param>
    /// <param name="agencySequenceNumber">Número de secuencia de la agencia</param>
    /// <returns>Número de secuencia formateado con 2 dígitos (D2)</returns>
    private static string GetNextSiteSequenceNumber(List<string> existingCodes, string agencySequenceNumber)
    {
        // Filtrar códigos de sitios que pertenecen a la agencia específica
        var agencySiteCodes = existingCodes.Where(c => c.StartsWith($"{agencySequenceNumber}-"))
                                          .Select(c => {
                                              var parts = c.Split('-');
                                              return parts.Length > 1 ? int.Parse(parts[^1]) : 0;
                                          })
                                          .DefaultIfEmpty(0)
                                          .Max();

        // Incrementar el número de secuencia y formatear con ceros a la izquierda (2 dígitos)
        return (agencySiteCodes + 1).ToString("D2");
    }

    /// <summary>
    /// Calcula el número de días operativos en un rango de fechas que coinciden con los días de la semana seleccionados.
    /// Sistema: 1=Lunes, 2=Martes, ..., 7=Domingo.
    /// </summary>
    public static int CalculateOperatingDaysCount(DateTime fromDate, DateTime toDate, List<int> selectedDayIds)
    {
        if (selectedDayIds == null || selectedDayIds.Count == 0)
            return 0;

        var start = fromDate.Date;
        var end = toDate.Date;
        if (start > end)
            (start, end) = (end, start);

        int count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            // .NET DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6 → Sistema: 1=Mon, ..., 7=Sun
            int systemDay = d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek;
            if (selectedDayIds.Contains(systemDay))
                count++;
        }

        return count;
    }
}
