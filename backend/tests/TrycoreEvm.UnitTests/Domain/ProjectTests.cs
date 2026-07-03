using FluentAssertions;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;
using TrycoreEvm.Domain.ValueObjects;
using Xunit;

namespace TrycoreEvm.UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Constructor_ConNombreValido_CreaElProyectoSinActividades()
    {
        var project = new Project("Implementación ERP");

        project.Name.Should().Be("Implementación ERP");
        project.Activities.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ConNombreVacio_LanzaExcepcionDeValidacion()
    {
        var act = () => new Project(string.Empty);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void CalculateConsolidatedIndicators_CuandoNoHayActividades_DevuelveIndicadoresEnCeroSinLanzarExcepcion()
    {
        var project = new Project("Proyecto sin actividades aún");

        var indicators = project.CalculateConsolidatedIndicators();

        indicators.PlannedValue.Should().Be(0m);
        indicators.EarnedValue.Should().Be(0m);
        indicators.ActualCost.Should().Be(0m);
        indicators.CostPerformanceIndex.Should().BeNull();
        indicators.SchedulePerformanceIndex.Should().BeNull();
        indicators.CostStatus.Should().Be(PerformanceStatus.NoAplica);
        indicators.ScheduleStatus.Should().Be(PerformanceStatus.NoAplica);
    }

    [Fact]
    public void CalculateConsolidatedIndicators_SumaCorrectamenteLosValoresDeVariasActividades()
    {
        var project = new Project("Proyecto con actividades");
        project.AddActivity("Actividad 1", 1000m, 50m, 40m, 400m);
        project.AddActivity("Actividad 2", 2000m, 100m, 80m, 1500m);

        var indicators = project.CalculateConsolidatedIndicators();

        indicators.PlannedValue.Should().Be(2500m);
        indicators.EarnedValue.Should().Be(2000m);
        indicators.ActualCost.Should().Be(1900m);
    }

    [Fact]
    public void AddActivity_AgregaLaActividadAlProyecto()
    {
        var project = new Project("Proyecto");

        var activity = project.AddActivity("Actividad 1", 1000m, 50m, 40m, 400m);

        project.Activities.Should().ContainSingle(a => a.Id == activity.Id);
    }


  
}
