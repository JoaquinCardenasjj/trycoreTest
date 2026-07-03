using Microsoft.EntityFrameworkCore;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Infrastructure.Persistence;

namespace TrycoreEvm.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvmDbContext _dbContext;

    public UnitOfWork(EvmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Log mínimo para identificar la entrada conflictiva
            foreach (var entry in ex.Entries)
            {
                var entityType = entry.Entity.GetType().Name;
                var keyValues = entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .Select(p => $"{p.Metadata.Name}={p.CurrentValue}")
                    .ToArray();

                // Reemplaza por tu logger, aquí Console para debug local

                var variable = $"Concurrency conflict on entity {entityType}. Keys: {string.Join(",", keyValues)}";

                Console.Error.WriteLine($"Concurrency conflict on entity {entityType}. Keys: {string.Join(",", keyValues)}");
            }

            throw;
        }
    }
}
