using TrycoreEvm.Application.Dtos;

namespace TrycoreEvm.Application.Services;

public interface IProjectService
{
    Task<IReadOnlyCollection<ProjectSummaryResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProjectDetailResponse> GetByIdAsync(Guid projectId, CancellationToken cancellationToken);

    Task<ProjectDetailResponse> CreateAsync(ProjectRequest request, CancellationToken cancellationToken);

    Task<ProjectDetailResponse> RenameAsync(Guid projectId, ProjectRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid projectId, CancellationToken cancellationToken);
}
