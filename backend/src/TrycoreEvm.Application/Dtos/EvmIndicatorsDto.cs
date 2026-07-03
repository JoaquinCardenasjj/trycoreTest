namespace TrycoreEvm.Application.Dtos;

/// <summary>
/// Representación serializable de los indicadores EVM, incluyendo la interpretación
/// textual de CPI y SPI que pide el reto ("bajo/sobre presupuesto", "adelantado/atrasado").
/// </summary>
public sealed record EvmIndicatorsDto(
    decimal PlannedValue,
    decimal EarnedValue,
    decimal ActualCost,
    decimal CostVariance,
    decimal ScheduleVariance,
    decimal? CostPerformanceIndex,
    decimal? SchedulePerformanceIndex,
    decimal? EstimateAtCompletion,
    decimal? VarianceAtCompletion,
    string CostInterpretation,
    string ScheduleInterpretation);
