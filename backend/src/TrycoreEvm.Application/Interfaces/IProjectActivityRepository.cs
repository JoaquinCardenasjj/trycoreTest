using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Application.Interfaces;

/// <summary>
/// Puerto de persistencia para la raíz de agregado Project. La capa de aplicación
/// depende únicamente de esta abstracción; la implementación concreta vive en Infrastructure.
/// </summary>
public interface IProjectActivityRepository
{
    Task<ProjectActivity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ProjectActivity>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(ProjectActivity project, CancellationToken cancellationToken);

    void Remove(ProjectActivity project);
}
