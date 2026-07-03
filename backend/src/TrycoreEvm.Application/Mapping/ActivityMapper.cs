using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Application.Mapping;

/// <summary>Traduce entidades <see cref="ProjectActivity"/> a su representación de API.</summary>
public static class ActivityMapper
{
    public static ActivityResponse ToResponse(ProjectActivity activity)
    {
        var indicators = EvmIndicatorsMapper.ToDto(activity.CalculateIndicators());

        return new ActivityResponse(
            Id: activity.Id,
            ProjectId: activity.ProjectId,
            Name: activity.Name,
            BudgetAtCompletion: activity.BudgetAtCompletion,
            PlannedPercentComplete: activity.PlannedPercentComplete,
            ActualPercentComplete: activity.ActualPercentComplete,
            ActualCost: activity.ActualCost,
            Indicators: indicators);
    }
}
