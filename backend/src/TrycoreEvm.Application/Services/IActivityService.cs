using TrycoreEvm.Application.Dtos;

namespace TrycoreEvm.Application.Services;

public interface IActivityService
{
    Task<ActivityResponse> AddAsync(Guid projectId, ActivityRequest request, CancellationToken cancellationToken);

    Task<ActivityResponse> UpdateAsync(Guid projectId, Guid activityId, ActivityRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid projectId, Guid activityId, CancellationToken cancellationToken);
}
