using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Domain.ValueObjects;

namespace TrycoreEvm.Application.Mapping;

/// <summary>
/// Traduce el resultado neutral del dominio (<see cref="EvmIndicators"/>) a un DTO con
/// interpretaciones legibles en español, tal como lo pide el reto: indicar si el proyecto
/// está bajo o sobre presupuesto, y adelantado o atrasado.
/// </summary>
public static class EvmIndicatorsMapper
{
    private const string CostoNoAplica = "Sin costo real registrado aún: no es posible evaluar el desempeño de costo.";
    private const string CostoBajoPresupuesto = "Bajo presupuesto: se está gastando menos de lo planificado para el avance logrado.";
    private const string CostoEnLinea = "Exactamente en el presupuesto planificado.";
    private const string CostoSobrePresupuesto = "Sobre presupuesto: se está gastando más de lo planificado para el avance logrado.";

    private const string CronogramaNoAplica = "Sin valor planificado registrado aún: no es posible evaluar el desempeño de cronograma.";
    private const string CronogramaAdelantado = "Adelantado: el avance real supera al avance planificado a la fecha de corte.";
    private const string CronogramaEnLinea = "Exactamente en el cronograma planificado.";
    private const string CronogramaAtrasado = "Atrasado: el avance real es menor al avance planificado a la fecha de corte.";

    public static EvmIndicatorsDto ToDto(EvmIndicators indicators)
    {
        return new EvmIndicatorsDto(
            PlannedValue: indicators.PlannedValue,
            EarnedValue: indicators.EarnedValue,
            ActualCost: indicators.ActualCost,
            CostVariance: indicators.CostVariance,
            ScheduleVariance: indicators.ScheduleVariance,
            CostPerformanceIndex: indicators.CostPerformanceIndex,
            SchedulePerformanceIndex: indicators.SchedulePerformanceIndex,
            EstimateAtCompletion: indicators.EstimateAtCompletion,
            VarianceAtCompletion: indicators.VarianceAtCompletion,
            CostInterpretation: InterpretCostStatus(indicators.CostStatus),
            ScheduleInterpretation: InterpretScheduleStatus(indicators.ScheduleStatus));
    }

    private static string InterpretCostStatus(PerformanceStatus status) => status switch
    {
        PerformanceStatus.NoAplica => CostoNoAplica,
        PerformanceStatus.Favorable => CostoBajoPresupuesto,
        PerformanceStatus.EnLinea => CostoEnLinea,
        PerformanceStatus.Desfavorable => CostoSobrePresupuesto,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado de costo no reconocido.")
    };

    private static string InterpretScheduleStatus(PerformanceStatus status) => status switch
    {
        PerformanceStatus.NoAplica => CronogramaNoAplica,
        PerformanceStatus.Favorable => CronogramaAdelantado,
        PerformanceStatus.EnLinea => CronogramaEnLinea,
        PerformanceStatus.Desfavorable => CronogramaAtrasado,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado de cronograma no reconocido.")
    };
}
