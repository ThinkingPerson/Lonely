using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using LonelyApi.DTOs;

namespace LonelyApi.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponse(false, "服务器内部错误");

        // 根据异常类型设置不同的错误信息
        if (exception is ArgumentNullException)
        {
            response.Message = "参数为空";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is ArgumentException)
        {
            response.Message = "参数无效: " + exception.Message;
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is NullReferenceException)
        {
            response.Message = "空指针异常";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        else if (exception is UnauthorizedAccessException)
        {
            response.Message = "未授权访问";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception is ForbiddenAccessException)
        {
            response.Message = "禁止访问";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        else if (exception is NotFoundException)
        {
            response.Message = "资源不存在";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

// 自定义异常类
public class NotFoundException : Exception
{
    public NotFoundException() : base() { }
    public NotFoundException(string message) : base(message) { }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base() { }
    public ForbiddenAccessException(string message) : base(message) { }
}