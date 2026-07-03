namespace TrycoreEvm.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una entidad o valor del dominio no cumple sus reglas de negocio.
/// </summary>
public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }
}
