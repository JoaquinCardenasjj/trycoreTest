using Microsoft.AspNetCore.Mvc;
using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Services;

namespace TrycoreEvm.Api.Controllers;

/// <summary>Gestiona proyectos y expone sus indicadores EVM consolidados.</summary>
[ApiController]
[Route("api/projects")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>Lista todos los proyectos con sus indicadores EVM consolidados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProjectSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProjectSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetAllAsync(cancellationToken);
        return Ok(projects);
    }

    /// <summary>Obtiene el detalle de un proyecto, incluyendo sus actividades e indicadores.</summary>
    [HttpGet("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailResponse>> GetById(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(projectId, cancellationToken);
        return Ok(project);
    }

    /// <summary>Crea un nuevo proyecto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDetailResponse>> Create([FromBody] ProjectRequest request, CancellationToken cancellationToken)
    {
        var createdProject = await _projectService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { projectId = createdProject.Id }, createdProject);
    }

    /// <summary>Renombra un proyecto existente.</summary>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailResponse>> Rename(Guid projectId, [FromBody] ProjectRequest request, CancellationToken cancellationToken)
    {
        var updatedProject = await _projectService.RenameAsync(projectId, request, cancellationToken);
        return Ok(updatedProject);
    }

    /// <summary>Elimina un proyecto y todas sus actividades.</summary>
    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid projectId, CancellationToken cancellationToken)
    {
        await _projectService.DeleteAsync(projectId, cancellationToken);
        return NoContent();
    }
}
