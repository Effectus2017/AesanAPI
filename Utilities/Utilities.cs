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
}
