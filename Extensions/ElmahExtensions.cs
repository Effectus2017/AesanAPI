using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions;

public static class ElmahExtensions
{
    public static async Task RaiseError(this HttpContext context, Exception exception)
    {
        if (exception != null)
        {
            var errorLog = context.RequestServices.GetService<ErrorLog>();
            if (errorLog != null)
            {
                await errorLog.LogAsync(new Error(exception, context));
            }
        }
    }
}