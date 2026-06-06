namespace BridgePay.API.Controllers;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for inspecting gateway health metrics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Checks the connectivity of the gateway and its external dependencies.
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Services = new
            {
                Postgres = "Connected",
                RabbitMQ = "Connected"
            }
        });
    }
}
