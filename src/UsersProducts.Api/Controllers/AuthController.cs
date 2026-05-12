using Microsoft.AspNetCore.Mvc;
using UsersProducts.Api.Contracts.Auth;
using UsersProducts.Api.Services.Auth;

namespace UsersProducts.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var authResponse = await _authService.RegisterAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(Register),
            new { id = authResponse.User.Id },
            authResponse
        );
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var authResponse = await _authService.LoginAsync(request, cancellationToken);

        return Ok(authResponse);
    }
}