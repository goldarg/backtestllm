using System.Net;
using System.Text.Json;
using api.Exceptions;


namespace api.Middleware;
/// <summary>
/// Este middleware no se ejecutara si estamos en desarrollo ya que UseDeveloperExceptionPage
/// captura las exceptiones y muestra la traza de error
/// </summary>
/// <param name="next"></param>
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception excp)
        {
            await ExceptionAsync(context, excp);
        }
    }

    private static Task ExceptionAsync(HttpContext context, Exception ex)
    {
        // Here, the HTTP codes will be determined based on exceptions
        HttpStatusCode statusCode;
        var message = "Unexpected error";

        // We need to identify the type of the exception
        var excpType = ex.GetType();

        // Let's check what kind of exceptions are passed
        if (excpType == typeof(BadRequestException))
        {
            statusCode = HttpStatusCode.BadRequest;
            message = ex.Message;
        }
        else if (excpType == typeof(NotFoundException))
        {
            statusCode = HttpStatusCode.NotFound;
            message = ex.Message;
        }
        else if (excpType == typeof(UnauthorizedException))
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = ex.Message;
        }
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            message = ex.Message;
        }

        var result = JsonSerializer.Serialize(new { message = message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(result);
    }
}