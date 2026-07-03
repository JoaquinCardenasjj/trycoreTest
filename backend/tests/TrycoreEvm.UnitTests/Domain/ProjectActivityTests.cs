using FluentAssertions;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;
using Xunit;

namespace TrycoreEvm.UnitTests.Domain;

public class ProjectActivityTests
{
    private static readonly Guid ProjectId = Guid.NewGuid();

    [Fact]
    public void Constructor_ConDatosValidos_CreaLaActividadCorrectamente()
    {
        var activity = new ProjectActivity(ProjectId, "Diseño de arquitectura", 1000m, 50m, 40m, 400m);

        activity.Name.Should().Be("Diseño de arquitectura");
        activity.BudgetAtCompletion.Should().Be(1000m);
        activity.ProjectId.Should().Be(ProjectId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ConNombreVacio_LanzaExcepcionDeValidacion(string invalidName)
    {
        var act = () => new ProjectActivity(ProjectId, invalidName, 1000m, 50m, 40m, 400m);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Constructor_ConPresupuestoNegativo_LanzaExcepcionDeValidacion()
    {
        var act = () => new ProjectActivity(ProjectId, "Actividad", -100m, 50m, 40m, 400m);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Constructor_ConCostoRealNegativo_LanzaExcepcionDeValidacion()
    {
        var act = () => new ProjectActivity(ProjectId, "Actividad", 1000m, 50m, 40m, -1m);

        act.Should().Throw<DomainValidationException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_ConPorcentajePlanificadoFueraDeRango_LanzaExcepcionDeValidacion(decimal invalidPercentage)
    {
        var act = () => new ProjectActivity(ProjectId, "Actividad", 1000m, invalidPercentage, 40m, 400m);

        act.Should().Throw<DomainValidationException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_ConPorcentajeRealFueraDeRango_LanzaExcepcionDeValidacion(decimal invalidPercentage)
    {
        var act = () => new ProjectActivity(ProjectId, "Actividad", 1000m, 50m, invalidPercentage, 400m);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void UpdateDetails_ConDatosValidos_ActualizaLaActividadYSusIndicadores()
    {
        var activity = new ProjectActivity(ProjectId, "Actividad", 1000m, 50m, 40m, 400m);

        activity.UpdateDetails("Actividad renombrada", 2000m, 60m, 60m, 1200m);

        activity.Name.Should().Be("Actividad renombrada");
        activity.BudgetAtCompletion.Should().Be(2000m);
        activity.CalculateIndicators().CostPerformanceIndex.Should().Be(1m);
    }

    [Fact]
    public void CalculateIndicators_DelegaEnLaCalculadoraEvmConLosDatosDeLaActividad()
    {
        var activity = new ProjectActivity(ProjectId, "Actividad", 1000m, 50m, 40m, 400m);

        var indicators = activity.CalculateIndicators();

        indicators.PlannedValue.Should().Be(500m);
        indicators.EarnedValue.Should().Be(400m);
    }
}
