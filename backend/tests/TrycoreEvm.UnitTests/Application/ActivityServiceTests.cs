using FluentAssertions;
using Moq;
using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Application.Services;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;
using Xunit;

namespace TrycoreEvm.UnitTests.Application;

public class ActivityServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ActivityService _sut;

    public ActivityServiceTests()
    {
        _sut = new ActivityService(_projectRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private void SetupExistingProject(Project project)
    {
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
    }

    [Fact]
    public async Task AddAsync_ConProyectoExistente_AgregaLaActividadYConfirmaLosCambios()
    {
        var project = new Project("Proyecto");
        SetupExistingProject(project);
        var request = new ActivityRequest("Actividad 1", 1000m, 50m, 40m, 400m);

        var result = await _sut.AddAsync(project.Id, request, CancellationToken.None);

        result.Name.Should().Be("Actividad 1");
        result.Indicators.PlannedValue.Should().Be(500m);
        project.Activities.Should().ContainSingle();
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ConProyectoInexistente_LanzaEntityNotFoundException()
    {
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);
        var request = new ActivityRequest("Actividad 1", 1000m, 50m, 40m, 400m);

        var act = () => _sut.AddAsync(Guid.NewGuid(), request, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ConActividadExistente_ActualizaSusDatosYConfirmaLosCambios()
    {
        var project = new Project("Proyecto");
        var activity = project.AddActivity("Actividad original", 1000m, 50m, 40m, 400m);
        SetupExistingProject(project);
        var request = new ActivityRequest("Actividad actualizada", 2000m, 60m, 60m, 1200m);

        var result = await _sut.UpdateAsync(project.Id, activity.Id, request, CancellationToken.None);

        result.Name.Should().Be("Actividad actualizada");
        result.BudgetAtCompletion.Should().Be(2000m);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConActividadInexistente_LanzaEntityNotFoundException()
    {
        var project = new Project("Proyecto");
        SetupExistingProject(project);
        var request = new ActivityRequest("Actividad", 1000m, 50m, 40m, 400m);

        var act = () => _sut.UpdateAsync(project.Id, Guid.NewGuid(), request, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ConActividadExistente_LaEliminaYConfirmaLosCambios()
    {
        var project = new Project("Proyecto");
        var activity = project.AddActivity("Actividad", 1000m, 50m, 40m, 400m);
        SetupExistingProject(project);

        await _sut.DeleteAsync(project.Id, activity.Id, CancellationToken.None);

        project.Activities.Should().BeEmpty();
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
