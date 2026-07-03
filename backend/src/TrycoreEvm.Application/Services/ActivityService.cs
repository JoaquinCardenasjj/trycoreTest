using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Application.Mapping;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;

namespace TrycoreEvm.Application.Services;

public sealed class ActivityService : IActivityService
{
    private readonly IProjectActivityRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivityService(IProjectActivityRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActivityResponse> AddAsync(Guid projectId, ActivityRequest request, CancellationToken cancellationToken)
    {
        var projectActivity = new ProjectActivity(projectId, request.Name,
            request.BudgetAtCompletion,
            request.PlannedPercentComplete,
            request.ActualPercentComplete,
            request.ActualCost);        
         await _projectRepository.AddAsync(projectActivity, cancellationToken
           );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ActivityMapper.ToResponse(projectActivity);
    }

    public async Task DeleteAsync(Guid projectId, Guid activityId, CancellationToken cancellationToken)
    {
        var activity = await _projectRepository.GetByIdAsync(activityId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ProjectActivity), activityId);

        if (activity.ProjectId != projectId)
        {
            throw new DomainValidationException("La actividad no pertenece al proyecto indicado.");
        }

        _projectRepository.Remove(activity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ActivityResponse> UpdateAsync(Guid projectId, Guid activityId, ActivityRequest request, CancellationToken cancellationToken)
    {
        var activity = await _projectRepository.GetByIdAsync(activityId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ProjectActivity), activityId);

        if (activity.ProjectId != projectId)
        {
            throw new DomainValidationException("La actividad no pertenece al proyecto indicado.");
        }

        activity.UpdateDetails(
            request.Name,
            request.BudgetAtCompletion,
            request.PlannedPercentComplete,
            request.ActualPercentComplete,
            request.ActualCost);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ActivityMapper.ToResponse(activity);
    }
}
