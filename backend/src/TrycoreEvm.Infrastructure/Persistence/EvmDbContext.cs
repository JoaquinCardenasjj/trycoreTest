using Microsoft.EntityFrameworkCore;
using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Infrastructure.Persistence;

public class EvmDbContext : DbContext
{
    public EvmDbContext(DbContextOptions<EvmDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectActivity> Activities => Set<ProjectActivity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EvmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
