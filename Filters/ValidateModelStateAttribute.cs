using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Models;
using Api.Models.Errors;

namespace Api.Filters;

/// <summary>
/// Filtro que valida el ModelState antes de ejecutar la acción.
/// Si el modelo es inválido, responde con 400 BadRequest y un ApiException estandarizado.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var message = string.Join(" | ", context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            var apiException = new ApiException(ErrorCode.VALIDATION_ERROR, message, 400);

            context.Result = new BadRequestObjectResult(apiException);
        }
    }
}
