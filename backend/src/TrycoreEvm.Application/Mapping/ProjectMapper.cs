using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Application.Mapping;

/// <summary>Traduce entidades <see cref="Project"/> a sus representaciones de API.</summary>
public static class ProjectMapper
{
    public static ProjectSummaryResponse ToSummaryResponse(Project project)
    {
        var consolidatedIndicators = EvmIndicatorsMapper.ToDto(project.CalculateConsolidatedIndicators());

        return new ProjectSummaryResponse(
            Id: project.Id,
            Name: project.Name,
            ActivityCount: project.Activities.Count,
            ConsolidatedIndicators: consolidatedIndicators);
    }

    public static ProjectDetailResponse ToDetailResponse(Project project)
    {
        var consolidatedIndicators = EvmIndicatorsMapper.ToDto(project.CalculateConsolidatedIndicators());
        var activities = project.Activities.Select(ActivityMapper.ToResponse).ToList();

        return new ProjectDetailResponse(
            Id: project.Id,
            Name: project.Name,
            ConsolidatedIndicators: consolidatedIndicators,
            Activities: activities);
    }
}
