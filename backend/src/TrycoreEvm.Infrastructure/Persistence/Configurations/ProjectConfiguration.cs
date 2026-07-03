using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    private const int NameMaxLength = 200;

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.HasMany(project => project.Activities)
            .WithOne()
            .HasForeignKey(activity => activity.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Project.Activities))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
