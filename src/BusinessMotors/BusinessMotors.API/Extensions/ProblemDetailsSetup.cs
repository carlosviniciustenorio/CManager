using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusinessMotors.API.Extensions;
public static class ProblemDetailsSetup
{
    public static void AddApiProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) =>
            {
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };

            options.MapExceptionToStatusCodeWithMessage<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
            options.MapExceptionToStatusCodeWithMessage<ArgumentException>(StatusCodes.Status400BadRequest);
            options.MapExceptionToStatusCodeWithMessage<ArgumentNullException>(StatusCodes.Status400BadRequest);
            options.MapExceptionToStatusCodeWithMessage<InvalidDataException>(StatusCodes.Status404NotFound);
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        })
        .AddProblemDetailsConventions();
    }

    public static void MapExceptionToStatusCodeWithMessage<TException>(this Hellang.Middleware.ProblemDetails.ProblemDetailsOptions options, int statusCode) where TException : Exception
    {
        options.Map<TException>(ex => new StatusCodeProblemDetails(statusCode)
        {
            Detail = ex.Message
        });
    }
}