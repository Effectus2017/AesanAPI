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
}
