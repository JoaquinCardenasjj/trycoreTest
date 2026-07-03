using Microsoft.AspNetCore.Mvc;
using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Services;

namespace TrycoreEvm.Api.Controllers;

/// <summary>Gestiona las actividades de un proyecto y expone sus indicadores EVM.</summary>
[ApiController]
[Route("api/projects/{projectId:guid}/activities")]
[Produces("application/json")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    /// <summary>Agrega una actividad a un proyecto existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityResponse>> Add(Guid projectId, [FromBody] ActivityRequest request, CancellationToken cancellationToken)
    {
        var createdActivity = await _activityService.AddAsync(projectId, request, cancellationToken);
        return CreatedAtAction(
            actionName: nameof(ProjectsController.GetById),
            controllerName: "Projects",
            routeValues: new { projectId },
            value: createdActivity);
    }

    /// <summary>Actualiza los datos de avance, presupuesto y costo de una actividad.</summary>
    [HttpPut("{activityId:guid}")]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityResponse>> Update(
        Guid projectId,
        Guid activityId,
        [FromBody] ActivityRequest request,
        CancellationToken cancellationToken)
    {
        var updatedActivity = await _activityService.UpdateAsync(projectId, activityId, request, cancellationToken);
        return Ok(updatedActivity);
    }

    /// <summary>Elimina una actividad de un proyecto.</summary>
    [HttpDelete("{activityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid projectId, Guid activityId, CancellationToken cancellationToken)
    {
        await _activityService.DeleteAsync(projectId, activityId, cancellationToken);
        return NoContent();
    }
}
