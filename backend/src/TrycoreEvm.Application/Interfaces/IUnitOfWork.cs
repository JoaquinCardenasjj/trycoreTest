namespace TrycoreEvm.Application.Interfaces;

/// <summary>
/// Abstrae la confirmación transaccional de los cambios realizados a través de los repositorios.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
