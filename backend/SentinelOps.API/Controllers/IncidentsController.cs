using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelOps.Application.Common.Models;
using SentinelOps.Application.Incidents.Commands;
using SentinelOps.Application.Incidents.Queries;
using SentinelOps.Domain.Enums;

namespace SentinelOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IncidentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns a filtered list of incidents.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncidents(
        [FromQuery] IncidentStatus? status,
        [FromQuery] Severity? severity)
    {
        var result = await _mediator.Send(new GetIncidentsQuery(status, severity));
        return Ok(result);
    }

    /// <summary>Returns a single incident by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncidentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIncident(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetIncidentByIdQuery(id));
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Creates a new incident and returns its Id.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentDto dto)
    {
        var command = new CreateIncidentCommand(
            dto.Title,
            dto.Description,
            dto.Severity,
            dto.AssignedToId,
            dto.SlaHours);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetIncident), new { id }, id);
    }

    /// <summary>Updates an existing incident.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentDto dto)
    {
        try
        {
            var command = new UpdateIncidentCommand(
                id,
                dto.Title,
                dto.Description,
                dto.Status,
                dto.Severity,
                dto.AssignedToId);

            await _mediator.Send(command);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Deletes an incident.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIncident(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteIncidentCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Triggers AI root-cause analysis for the incident.</summary>
    [HttpPost("{id:guid}/analyze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnalyzeIncident(Guid id)
    {
        try
        {
            await _mediator.Send(new AnalyzeIncidentWithAiCommand(id));
            return Ok(new { message = "AI analysis started" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
