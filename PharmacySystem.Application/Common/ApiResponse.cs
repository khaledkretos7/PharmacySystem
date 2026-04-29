namespace PharmacySystem.Application.Common;

/// <summary>
/// Standardized API response envelope used by all controllers.
/// Ensures a consistent JSON shape across every endpoint.
/// </summary>
public class ApiResponse<T>
{
    public bool   Success    { get; init; }
    public string Message    { get; init; } = string.Empty;
    public T?     Data       { get; init; }
    public object? Errors    { get; init; }
    public int    StatusCode { get; init; }

    // ── Factory helpers ────────────────────────────────────────────────────

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data, StatusCode = 200 };

    public static ApiResponse<T> Created(T data, string message = "Created successfully") =>
        new() { Success = true, Message = message, Data = data, StatusCode = 201 };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, object? errors = null) =>
        new() { Success = false, Message = message, StatusCode = statusCode, Errors = errors };

    public static ApiResponse<T> NotFound(string message = "Resource not found") =>
        new() { Success = false, Message = message, StatusCode = 404 };

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized") =>
        new() { Success = false, Message = message, StatusCode = 401 };

    public static ApiResponse<T> Forbidden(string message = "Access denied") =>
        new() { Success = false, Message = message, StatusCode = 403 };

    public static ApiResponse<T> ServerError(string message = "An internal error occurred") =>
        new() { Success = false, Message = message, StatusCode = 500 };
}

/// <summary>Non-generic shorthand for void responses.</summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse<object> Ok(string message = "Success") =>
        new() { Success = true, Message = message, StatusCode = 200 };
}
