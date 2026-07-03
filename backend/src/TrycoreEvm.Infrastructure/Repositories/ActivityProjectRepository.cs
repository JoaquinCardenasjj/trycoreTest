using Microsoft.EntityFrameworkCore;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Infrastructure.Persistence;

namespace TrycoreEvm.Infrastructure.Repositories;

public class ActivityProjectRepository : IProjectActivityRepository
{
    private readonly EvmDbContext _dbContext;

    public ActivityProjectRepository(EvmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectActivity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Activities            
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProjectActivity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Activities            
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ProjectActivity project, CancellationToken cancellationToken)
    {
        await _dbContext.Activities.AddAsync(project, cancellationToken);
    }


    public void Remove(ProjectActivity project)
    {
        _dbContext.Activities.Remove(project);
    }
}
