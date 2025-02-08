using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

    public static string GetUrl(HttpRequest request)
    {
        string scheame = request.Scheme;
        string host = request.Host.Value;
        string fullURL = string.Format($"{scheame}//{host}");

        return fullURL;
    }

    public static string GetUrl(ApplicationSettings appSettings)
    {

#if DEBUG
        return appSettings.LocalURL;
#endif

#if STAGING
        return appSettings.StagingURL;
#endif

#if RELEASE
        return appSettings.ProduccionURL;
#endif
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
        var programParts = programIds.OrderBy(p => p)
                                   .Select(p => $"P{p}")
                                   .ToList();
        string programsStr = string.Join("-", programParts);

        // Obtener el año actual
        string year = DateTime.Now.Year.ToString();

        // Obtener el siguiente número de secuencia
        string sequence = GetNextSequenceNumber(existingCodes, year);

        // Construir el código final
        return $"{uniqueInitials}{randomNum}-{programsStr}-{year}-{sequence}";
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
}
