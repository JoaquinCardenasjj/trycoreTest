using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Application.Mapping;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;

namespace TrycoreEvm.Application.Services;

/// <summary>
/// Orquesta los casos de uso de Project: valida la existencia de la entidad, delega las
/// reglas de negocio en el dominio y traduce el resultado a DTOs para el API.
/// </summary>
public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<ProjectSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);
        return projects.Select(ProjectMapper.ToSummaryResponse).ToList();
    }

    public async Task<ProjectDetailResponse> GetByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await GetProjectOrThrowAsync(projectId, cancellationToken);
        return ProjectMapper.ToDetailResponse(project);
    }

    public async Task<ProjectDetailResponse> CreateAsync(ProjectRequest request, CancellationToken cancellationToken)
    {
        var project = new Project(request.Name);
        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ProjectMapper.ToDetailResponse(project);
    }

    public async Task<ProjectDetailResponse> RenameAsync(Guid projectId, ProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await GetProjectOrThrowAsync(projectId, cancellationToken);
        project.Rename(request.Name);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ProjectMapper.ToDetailResponse(project);
    }

    public async Task DeleteAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await GetProjectOrThrowAsync(projectId, cancellationToken);
        _projectRepository.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Project> GetProjectOrThrowAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _projectRepository.GetByIdAsync(projectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);
    }
}
