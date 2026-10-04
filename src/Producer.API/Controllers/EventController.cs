using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Producer.API.Models;
using Producer.API.Services;

namespace Producer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventController : ControllerBase
{
    private readonly EventHubProducerService _producerService;
    private readonly ILogger<EventController> _logger;

    public EventController(EventHubProducerService producerService, ILogger<EventController> logger)
    {
        _producerService = producerService;
        _logger = logger;
    }

    [HttpPost("publish")]
    public async Task<IActionResult> PublishEvent([FromBody] OrderEvent orderEvent)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _producerService.SendEventAsync(orderEvent);
        return Ok(new { message = "Event published successfully", eventId = orderEvent.EventId });
    }

    [AllowAnonymous]
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}