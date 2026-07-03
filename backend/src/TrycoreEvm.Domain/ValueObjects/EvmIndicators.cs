namespace TrycoreEvm.Domain.ValueObjects;

/// <summary>
/// Conjunto inmutable de indicadores de Valor Ganado (EVM) resultantes de un cálculo,
/// ya sea para una actividad individual o para la consolidación de un proyecto completo.
/// Los índices y estimaciones son nulos cuando no pueden calcularse (división por cero),
/// lo cual es un resultado válido del dominio, no un error.
/// </summary>
public sealed record EvmIndicators(
    decimal PlannedValue,
    decimal EarnedValue,
    decimal ActualCost,
    decimal CostVariance,
    decimal ScheduleVariance,
    decimal? CostPerformanceIndex,
    decimal? SchedulePerformanceIndex,
    decimal? EstimateAtCompletion,
    decimal? VarianceAtCompletion,
    PerformanceStatus CostStatus,
    PerformanceStatus ScheduleStatus);
