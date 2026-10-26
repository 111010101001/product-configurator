using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ApiExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails? problem = exception switch
            {
                ConflictException e => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = e.Message,
                },
                DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "A record with the same unique value already exists.",
                },
                ConfigurationInvalidException e => new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid configuration",
                    Detail = e.Message,
                    Extensions = { ["violations"] = e.Violations },
                },
                _ => null,
            };

            if (problem is null)
                return false;   // not ours: the default 500

            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            return await problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problem,
            });
        }

    }
}
