using System.Globalization;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using BlogBackend.Exceptions;
using BlogBackend.Models.Info;

namespace BlogBackend.MiddleWares;

//https://www.youtube.com/watch?v=czkT9zySuDU&ab_channel=PlatinumDEV
public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next) =>
        _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception e)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = String.Empty;
        switch (e)
        {
            case InvalidCredentialException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new MessageResponse
                {
                    Status = ((int)code).ToString(),
                    Message = e.Message
                });
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new MessageResponse
                {
                    Status = ((int)code).ToString(),
                    Message = e.Message
                });
                break;
            case ResourceNotFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new MessageResponse
                {
                    Status = ((int)code).ToString(),
                    Message = e.Message
                });
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new MessageResponse
                {
                    Status = ((int)code).ToString(),
                    Message = e.Message
                });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}