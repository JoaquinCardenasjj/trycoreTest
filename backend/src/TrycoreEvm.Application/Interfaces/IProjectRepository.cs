using TrycoreEvm.Domain.Entities;

namespace TrycoreEvm.Application.Interfaces;

/// <summary>
/// Puerto de persistencia para la raíz de agregado Project. La capa de aplicación
/// depende únicamente de esta abstracción; la implementación concreta vive en Infrastructure.
/// </summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Project project, CancellationToken cancellationToken);

    void Remove(Project project);
}
