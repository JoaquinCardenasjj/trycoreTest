namespace TrycoreEvm.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se solicita una entidad que no existe en el sistema.
/// </summary>
public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object id)
        : base($"No se encontró {entityName} con identificador '{id}'.")
    {
    }
}
