using Microsoft.EntityFrameworkCore;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Infrastructure.Persistence;

namespace TrycoreEvm.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly EvmDbContext _dbContext;

    public ProjectRepository(EvmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Include(project => project.Activities)
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Include(project => project.Activities)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public void Remove(Project project)
    {
        _dbContext.Projects.Remove(project);
    }
}
