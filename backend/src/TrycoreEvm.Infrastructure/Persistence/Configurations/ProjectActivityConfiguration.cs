using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Infrastructure.Persistence.Configurations;

public class ProjectActivityConfiguration : IEntityTypeConfiguration<ProjectActivity>
{
    private const int NameMaxLength = 200;
    private const string MonetaryColumnType = "numeric(18,2)";
    private const string PercentageColumnType = "numeric(5,2)";

    public void Configure(EntityTypeBuilder<ProjectActivity> builder)
    {
        builder.ToTable("project_activities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(activity => activity.BudgetAtCompletion)
            .HasColumnType(MonetaryColumnType);

        builder.Property(activity => activity.ActualCost)
            .HasColumnType(MonetaryColumnType);

        builder.Property(activity => activity.PlannedPercentComplete)
            .HasColumnType(PercentageColumnType);

        builder.Property(activity => activity.ActualPercentComplete)
            .HasColumnType(PercentageColumnType);

        builder.Property(activity => activity.ProjectId)
            .IsRequired();
    }
}
