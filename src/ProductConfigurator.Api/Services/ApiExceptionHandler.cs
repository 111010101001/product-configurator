using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProductConfigurator.Api.Services
{
    public class ApiExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ConflictException && exception is not DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } })
                return false;   // not ours: the default handling turns it into a generic 500


            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            return await problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = exception is ConflictException c
                        ? c.Message
                        : "A record with the same unique value already exists.",
                },
            });
        }
    }
}
