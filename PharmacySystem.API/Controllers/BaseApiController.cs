using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PharmacySystem.API.Controllers;

/// <summary>Base controller providing current user helpers to all controllers.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>Returns the Id of the currently authenticated user.</summary>
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

    /// <summary>Returns the role of the currently authenticated user.</summary>
    protected string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
