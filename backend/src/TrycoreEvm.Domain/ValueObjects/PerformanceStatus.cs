namespace TrycoreEvm.Domain.ValueObjects;

/// <summary>
/// Estado de desempeño resultante de comparar un índice EVM (CPI o SPI) contra su valor objetivo.
/// El significado concreto (costo o cronograma) lo interpreta la capa de aplicación.
/// </summary>
public enum PerformanceStatus
{
    /// <summary>No se puede determinar el estado porque el denominador del índice es cero.</summary>
    NoAplica,

    /// <summary>El índice es menor al objetivo: sobre presupuesto o atrasado.</summary>
    Desfavorable,

    /// <summary>El índice es igual al objetivo: exactamente en línea con lo planificado.</summary>
    EnLinea,

    /// <summary>El índice es mayor al objetivo: bajo presupuesto o adelantado.</summary>
    Favorable
}
