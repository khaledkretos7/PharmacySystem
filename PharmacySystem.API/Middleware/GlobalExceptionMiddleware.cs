using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Domain.Exceptions;

namespace PharmacySystem.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            await HandleAsync(ctx, ex);
        }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        ctx.Response.ContentType = "application/json";

        ApiResponse<object> response;

        switch (ex)
        {
            case NotFoundException e:
                ctx.Response.StatusCode = 404;
                response = ApiResponse<object>.NotFound(e.Message);
                break;

            case DuplicateEntryException e:
                ctx.Response.StatusCode = 409;
                response = ApiResponse<object>.Fail(e.Message, 409);
                break;

            case InsufficientStockException e:
                ctx.Response.StatusCode = 422;
                response = ApiResponse<object>.Fail(e.Message, 422);
                break;

            case BusinessRuleException e:
                ctx.Response.StatusCode = 422;
                response = ApiResponse<object>.Fail(e.Message, 422);
                break;

            case InvalidOperationDomainException e:
                ctx.Response.StatusCode = 400;
                response = ApiResponse<object>.Fail(e.Message, 400);
                break;

            case ValidationException e:
                ctx.Response.StatusCode = 400;
                var errors = e.Errors
                    .GroupBy(err => err.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage).ToArray());
                response = ApiResponse<object>.Fail("Validation failed.", 400, errors);
                break;

            case UnauthorizedAccessException e:
                ctx.Response.StatusCode = 401;
                response = ApiResponse<object>.Unauthorized(e.Message);
                break;

            default:
                ctx.Response.StatusCode = 500;
                response = ApiResponse<object>.ServerError(
                    "An unexpected error occurred. Please try again later.");
                break;
        }

        await ctx.Response.WriteAsync(
            JsonSerializer.Serialize(response, _jsonOptions));
    }
}

// ── Extension for clean use in Program.cs ────────────────────────────────────
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}
