using FluentAssertions;
using TrycoreEvm.Domain.Services;
using TrycoreEvm.Domain.ValueObjects;
using Xunit;

namespace TrycoreEvm.UnitTests.Domain;

public class EvmCalculatorTests
{
    [Fact]
    public void CalculateForActivity_ConEscenarioNormal_CalculaTodosLosIndicadoresCorrectamente()
    {
        var indicators = EvmCalculator.CalculateForActivity(
            budgetAtCompletion: 1000m,
            plannedPercentComplete: 50m,
            actualPercentComplete: 40m,
            actualCost: 400m);

        indicators.PlannedValue.Should().Be(500m);
        indicators.EarnedValue.Should().Be(400m);
        indicators.CostVariance.Should().Be(0m);
        indicators.ScheduleVariance.Should().Be(-100m);
        indicators.CostPerformanceIndex.Should().Be(1m);
        indicators.SchedulePerformanceIndex.Should().Be(0.8m);
        indicators.EstimateAtCompletion.Should().Be(1000m);
        indicators.VarianceAtCompletion.Should().Be(0m);
        indicators.CostStatus.Should().Be(PerformanceStatus.EnLinea);
        indicators.ScheduleStatus.Should().Be(PerformanceStatus.Desfavorable);
    }

    [Fact]
    public void CalculateForActivity_CuandoElCostoRealEsCero_NoLanzaExcepcionYCpiQuedaIndefinido()
    {
        var indicators = EvmCalculator.CalculateForActivity(
            budgetAtCompletion: 1000m,
            plannedPercentComplete: 40m,
            actualPercentComplete: 20m,
            actualCost: 0m);

        indicators.PlannedValue.Should().Be(400m);
        indicators.EarnedValue.Should().Be(200m);
        indicators.CostVariance.Should().Be(200m);
        indicators.ScheduleVariance.Should().Be(-200m);
        indicators.CostPerformanceIndex.Should().BeNull();
        indicators.SchedulePerformanceIndex.Should().Be(0.5m);
        indicators.EstimateAtCompletion.Should().Be(800m);
        indicators.VarianceAtCompletion.Should().Be(200m);
        indicators.CostStatus.Should().Be(PerformanceStatus.NoAplica);
        indicators.ScheduleStatus.Should().Be(PerformanceStatus.Desfavorable);
    }

    [Fact]
    public void CalculateForActivity_CuandoElAvanceRealEsCero_NoLanzaExcepcionAlEstimarElCosto()
    {
        var indicators = EvmCalculator.CalculateForActivity(
            budgetAtCompletion: 1000m,
            plannedPercentComplete: 50m,
            actualPercentComplete: 0m,
            actualCost: 100m);

        indicators.EarnedValue.Should().Be(0m);
        indicators.CostVariance.Should().Be(-100m);
        indicators.ScheduleVariance.Should().Be(-500m);
        indicators.CostPerformanceIndex.Should().Be(0m);
        indicators.SchedulePerformanceIndex.Should().Be(0m);
        indicators.EstimateAtCompletion.Should().Be(1100m);
        indicators.VarianceAtCompletion.Should().Be(-100m);
        indicators.CostStatus.Should().Be(PerformanceStatus.Desfavorable);
        indicators.ScheduleStatus.Should().Be(PerformanceStatus.Desfavorable);
    }

    [Fact]
    public void CalculateForActivity_CuandoElAvanceRealYElCostoRealSonCero_NoLanzaExcepcion()
    {
        var indicators = EvmCalculator.CalculateForActivity(
            budgetAtCompletion: 1000m,
            plannedPercentComplete: 30m,
            actualPercentComplete: 0m,
            actualCost: 0m);

        indicators.EarnedValue.Should().Be(0m);
        indicators.CostPerformanceIndex.Should().BeNull();
        indicators.EstimateAtCompletion.Should().Be(1000m);
        indicators.CostStatus.Should().Be(PerformanceStatus.NoAplica);
    }

    [Fact]
    public void CalculateConsolidated_CuandoElProyectoNoTieneActividades_DevuelveIndicadoresEnCeroSinLanzarExcepcion()
    {
        var indicators = EvmCalculator.CalculateConsolidated(
            totalBudgetAtCompletion: 0m,
            totalPlannedValue: 0m,
            totalEarnedValue: 0m,
            totalActualCost: 0m);

        indicators.PlannedValue.Should().Be(0m);
        indicators.EarnedValue.Should().Be(0m);
        indicators.CostVariance.Should().Be(0m);
        indicators.ScheduleVariance.Should().Be(0m);
        indicators.CostPerformanceIndex.Should().BeNull();
        indicators.SchedulePerformanceIndex.Should().BeNull();
        indicators.EstimateAtCompletion.Should().Be(0m);
        indicators.VarianceAtCompletion.Should().Be(0m);
        indicators.CostStatus.Should().Be(PerformanceStatus.NoAplica);
        indicators.ScheduleStatus.Should().Be(PerformanceStatus.NoAplica);
    }

    [Fact]
    public void CalculateConsolidated_SumandoVariasActividades_ProduceElMismoResultadoQueCalcularUnaSolaVezSobreLosTotales()
    {
        var actividadUno = EvmCalculator.CalculateForActivity(1000m, 50m, 40m, 400m);
        var actividadDos = EvmCalculator.CalculateForActivity(2000m, 100m, 80m, 1500m);

        var consolidado = EvmCalculator.CalculateConsolidated(
            totalBudgetAtCompletion: 3000m,
            totalPlannedValue: actividadUno.PlannedValue + actividadDos.PlannedValue,
            totalEarnedValue: actividadUno.EarnedValue + actividadDos.EarnedValue,
            totalActualCost: actividadUno.ActualCost + actividadDos.ActualCost);

        consolidado.PlannedValue.Should().Be(2500m);
        consolidado.EarnedValue.Should().Be(2000m);
        consolidado.ActualCost.Should().Be(1900m);
        consolidado.CostVariance.Should().Be(100m);
        consolidado.ScheduleVariance.Should().Be(-500m);
    }

    [Theory]
    [InlineData(100, 50, 50, 50, PerformanceStatus.EnLinea)]
    [InlineData(100, 50, 60, 50, PerformanceStatus.Favorable)]
    [InlineData(100, 50, 40, 50, PerformanceStatus.Desfavorable)]
    public void CalculateForActivity_ElEstadoDeCostoReflejaCorrectamenteElValorDelCpi(
        decimal budgetAtCompletion,
        decimal plannedPercentComplete,
        decimal actualPercentComplete,
        decimal actualCost,
        PerformanceStatus expectedStatus)
    {
        var indicators = EvmCalculator.CalculateForActivity(budgetAtCompletion, plannedPercentComplete, actualPercentComplete, actualCost);

        indicators.CostStatus.Should().Be(expectedStatus);
    }

    [Fact]
    public void CalculateForActivity_ConPorcentajesEnCienPorCientoYCostoIgualAlPresupuesto_QuedaExactamenteEnLinea()
    {
        var indicators = EvmCalculator.CalculateForActivity(
            budgetAtCompletion: 5000m,
            plannedPercentComplete: 100m,
            actualPercentComplete: 100m,
            actualCost: 5000m);

        indicators.CostVariance.Should().Be(0m);
        indicators.ScheduleVariance.Should().Be(0m);
        indicators.CostPerformanceIndex.Should().Be(1m);
        indicators.SchedulePerformanceIndex.Should().Be(1m);
        indicators.EstimateAtCompletion.Should().Be(5000m);
        indicators.VarianceAtCompletion.Should().Be(0m);
    }
}
