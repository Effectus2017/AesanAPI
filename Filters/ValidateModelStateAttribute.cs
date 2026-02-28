using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

/// <summary>
/// Filtro que valida el ModelState antes de ejecutar la acción.
/// Si el modelo es inválido, responde con 400 BadRequest y el listado de errores
/// usando el mismo formato que Utilities.GetErrorListFromModelState.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateModelStateAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        context.Result = new BadRequestObjectResult(Utilities.GetErrorListFromModelState(context.ModelState));
    }
}
