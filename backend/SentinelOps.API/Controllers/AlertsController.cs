using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelOps.Application.Alerts.Commands;
using SentinelOps.Application.Alerts.Queries;
using SentinelOps.Application.Common.Models;

namespace SentinelOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns all active alerts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var result = await _mediator.Send(new GetActiveAlertsQuery());
        return Ok(result);
    }

    /// <summary>Creates a new alert.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
    {
        var command = new CreateAlertCommand(
            dto.Title,
            dto.Message,
            dto.Source,
            dto.Severity,
            dto.MetricValue,
            dto.Threshold);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetActiveAlerts), new { id }, id);
    }

    /// <summary>Acknowledges an alert by id.</summary>
    [HttpPut("{id:guid}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcknowledgeAlert(Guid id)
    {
        try
        {
            await _mediator.Send(new AcknowledgeAlertCommand(id));
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
