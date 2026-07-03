using TrycoreEvm.Domain.Common;
using TrycoreEvm.Domain.ValueObjects;

namespace TrycoreEvm.Domain.Services;

/// <summary>
/// Calculadora pura de indicadores de Valor Ganado (Earned Value Management).
/// No tiene dependencias externas ni estado: dadas las mismas entradas, siempre
/// produce las mismas salidas, lo que la hace trivial de cubrir con pruebas unitarias.
/// </summary>
public static class EvmCalculator
{
    /// <summary>
    /// Calcula los indicadores EVM de una actividad a partir de su presupuesto y sus
    /// porcentajes de avance planificado y real (expresados en el rango 0-100).
    /// </summary>
    public static EvmIndicators CalculateForActivity(
        decimal budgetAtCompletion,
        decimal plannedPercentComplete,
        decimal actualPercentComplete,
        decimal actualCost)
    {
        var plannedValue = ToAbsoluteValue(plannedPercentComplete, budgetAtCompletion);
        var earnedValue = ToAbsoluteValue(actualPercentComplete, budgetAtCompletion);

        return CalculateFromAbsoluteValues(budgetAtCompletion, plannedValue, earnedValue, actualCost);
    }

    /// <summary>
    /// Calcula los indicadores EVM consolidados de un proyecto a partir de la suma de los
    /// valores absolutos (BAC, PV, EV, AC) de todas sus actividades. Si no hay actividades,
    /// todas las sumas son cero y el método sigue devolviendo un resultado válido, sin lanzar
    /// excepciones ni dividir por cero.
    /// </summary>
    public static EvmIndicators CalculateConsolidated(
        decimal totalBudgetAtCompletion,
        decimal totalPlannedValue,
        decimal totalEarnedValue,
        decimal totalActualCost)
    {
        return CalculateFromAbsoluteValues(totalBudgetAtCompletion, totalPlannedValue, totalEarnedValue, totalActualCost);
    }

    private static EvmIndicators CalculateFromAbsoluteValues(
        decimal budgetAtCompletion,
        decimal plannedValue,
        decimal earnedValue,
        decimal actualCost)
    {
        var costVariance = earnedValue - actualCost;
        var scheduleVariance = earnedValue - plannedValue;

        var costPerformanceIndex = CalculatePerformanceIndex(earnedValue, actualCost);
        var schedulePerformanceIndex = CalculatePerformanceIndex(earnedValue, plannedValue);

        var estimateAtCompletion = CalculateEstimateAtCompletion(budgetAtCompletion, actualCost, earnedValue, costPerformanceIndex);
        var varianceAtCompletion = CalculateVarianceAtCompletion(budgetAtCompletion, estimateAtCompletion);

        return new EvmIndicators(
            PlannedValue: plannedValue,
            EarnedValue: earnedValue,
            ActualCost: actualCost,
            CostVariance: costVariance,
            ScheduleVariance: scheduleVariance,
            CostPerformanceIndex: costPerformanceIndex,
            SchedulePerformanceIndex: schedulePerformanceIndex,
            EstimateAtCompletion: estimateAtCompletion,
            VarianceAtCompletion: varianceAtCompletion,
            CostStatus: DetermineStatus(costPerformanceIndex),
            ScheduleStatus: DetermineStatus(schedulePerformanceIndex));
    }

    private static decimal ToAbsoluteValue(decimal percentage, decimal budgetAtCompletion)
    {
        var fraction = percentage / EvmConstants.PercentageDivisor;
        return fraction * budgetAtCompletion;
    }

    /// <summary>
    /// CPI = EV / AC y SPI = EV / PV comparten la misma forma (numerador / denominador).
    /// Cuando el denominador es cero el índice queda indefinido (null) en lugar de lanzar
    /// una excepción de división por cero: es un resultado de negocio válido (por ejemplo,
    /// una actividad que aún no ha incurrido en costos reales).
    /// </summary>
    private static decimal? CalculatePerformanceIndex(decimal earnedValue, decimal denominator)
    {
        if (denominator == EvmConstants.Zero)
        {
            return null;
        }

        return earnedValue / denominator;
    }

    /// <summary>
    /// EAC = BAC / CPI. Cuando el CPI no puede calcularse (aún no hay costo real incurrido),
    /// se usa como respaldo la estimación clásica "costo real + trabajo restante al presupuesto
    /// planificado" (AC + (BAC - EV)), que para AC = 0 y EV = 0 equivale al propio BAC.
    /// </summary>
    private static decimal? CalculateEstimateAtCompletion(
        decimal budgetAtCompletion,
        decimal actualCost,
        decimal earnedValue,
        decimal? costPerformanceIndex)
    {
        var costPerformanceIndexIsUsable = costPerformanceIndex.HasValue && costPerformanceIndex.Value != EvmConstants.Zero;

        if (!costPerformanceIndexIsUsable)
        {
            return actualCost + (budgetAtCompletion - earnedValue);
        }

        return budgetAtCompletion / costPerformanceIndex!.Value;
    }

    private static decimal? CalculateVarianceAtCompletion(decimal budgetAtCompletion, decimal? estimateAtCompletion)
    {
        return estimateAtCompletion.HasValue
            ? budgetAtCompletion - estimateAtCompletion.Value
            : null;
    }

    private static PerformanceStatus DetermineStatus(decimal? performanceIndex)
    {
        if (!performanceIndex.HasValue)
        {
            return PerformanceStatus.NoAplica;
        }

        if (performanceIndex.Value > EvmConstants.PerformanceIndexTarget)
        {
            return PerformanceStatus.Favorable;
        }

        return performanceIndex.Value < EvmConstants.PerformanceIndexTarget
            ? PerformanceStatus.Desfavorable
            : PerformanceStatus.EnLinea;
    }
}
