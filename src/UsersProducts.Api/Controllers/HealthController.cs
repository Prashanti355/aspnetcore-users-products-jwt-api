using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using UsersProducts.Api.Contracts.Health;
using Microsoft.AspNetCore.Authorization;

namespace UsersProducts.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    private readonly IHostEnvironment _environment;

    public HealthController(IHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> GetHealth()
    {
        var response = new HealthResponse(
            Status: "Healthy",
            Service: "UsersProducts.Api",
            Environment: _environment.EnvironmentName,
            TimestampUtc: DateTime.UtcNow
        );

        return Ok(response);
    }
}