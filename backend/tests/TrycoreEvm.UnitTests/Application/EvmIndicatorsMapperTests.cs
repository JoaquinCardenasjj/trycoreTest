using FluentAssertions;
using TrycoreEvm.Application.Mapping;
using TrycoreEvm.Domain.ValueObjects;
using Xunit;

namespace TrycoreEvm.UnitTests.Application;

public class EvmIndicatorsMapperTests
{
    private static EvmIndicators BuildIndicators(PerformanceStatus costStatus, PerformanceStatus scheduleStatus)
    {
        return new EvmIndicators(
            PlannedValue: 100m,
            EarnedValue: 100m,
            ActualCost: 100m,
            CostVariance: 0m,
            ScheduleVariance: 0m,
            CostPerformanceIndex: 1m,
            SchedulePerformanceIndex: 1m,
            EstimateAtCompletion: 100m,
            VarianceAtCompletion: 0m,
            CostStatus: costStatus,
            ScheduleStatus: scheduleStatus);
    }

    [Fact]
    public void ToDto_ConEstadoFavorable_DevuelveInterpretacionDeBajoPresupuestoYAdelantado()
    {
        var dto = EvmIndicatorsMapper.ToDto(BuildIndicators(PerformanceStatus.Favorable, PerformanceStatus.Favorable));

        dto.CostInterpretation.Should().Contain("Bajo presupuesto");
        dto.ScheduleInterpretation.Should().Contain("Adelantado");
    }

    [Fact]
    public void ToDto_ConEstadoDesfavorable_DevuelveInterpretacionDeSobrePresupuestoYAtrasado()
    {
        var dto = EvmIndicatorsMapper.ToDto(BuildIndicators(PerformanceStatus.Desfavorable, PerformanceStatus.Desfavorable));

        dto.CostInterpretation.Should().Contain("Sobre presupuesto");
        dto.ScheduleInterpretation.Should().Contain("Atrasado");
    }

    [Fact]
    public void ToDto_ConEstadoNoAplica_DevuelveInterpretacionQueIndicaFaltaDeDatos()
    {
        var dto = EvmIndicatorsMapper.ToDto(BuildIndicators(PerformanceStatus.NoAplica, PerformanceStatus.NoAplica));

        dto.CostInterpretation.Should().Contain("Sin costo real registrado");
        dto.ScheduleInterpretation.Should().Contain("Sin valor planificado registrado");
    }
}
