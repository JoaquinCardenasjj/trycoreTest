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
    private readonly Mock<IProjectActivityRepository> _projectActivityRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ActivityService _sut;

    public ActivityServiceTests()
    {
        _sut = new ActivityService(_projectActivityRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private void SetupExistingActivity(ProjectActivity activity)
    {
        _projectActivityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);
    }

    [Fact]
    public async Task AddAsync_ConDatosValidos_CreaLaActividadYConfirmaLosCambios()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ActivityRequest("Actividad 1", 1000m, 50m, 40m, 400m);

        // Act
        var result = await _sut.AddAsync(projectId, request, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Actividad 1");
        _projectActivityRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProjectActivity>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConActividadExistente_ActualizaSusDatosYConfirmaLosCambios()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var activity = new ProjectActivity(projectId, "Actividad original", 1000m, 50m, 40m, 400m);
        SetupExistingActivity(activity);

        var request = new ActivityRequest("Actividad actualizada", 2000m, 60m, 60m, 1200m);

        // Act
        var result = await _sut.UpdateAsync(projectId, activity.Id, request, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Actividad actualizada");
        result.BudgetAtCompletion.Should().Be(2000m);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConActividadInexistente_LanzaEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectActivityRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectActivity?)null);

        var request = new ActivityRequest("Actividad", 1000m, 50m, 40m, 400m);

        // Act
        var act = () => _sut.UpdateAsync(projectId, Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_CuandoActividadNoPerteneceAlProyecto_LanzaDomainValidationException()
    {
        // Arrange
        var proyectoRealId = Guid.NewGuid();
        var proyectoIncorrectoId = Guid.NewGuid();

        var activity = new ProjectActivity(proyectoRealId, "Actividad", 1000m, 50m, 40m, 400m);
        SetupExistingActivity(activity);

        var request = new ActivityRequest("Actividad", 1000m, 50m, 40m, 400m);

        // Act
        var act = () => _sut.UpdateAsync(proyectoIncorrectoId, activity.Id, request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("La actividad no pertenece al proyecto indicado.");
    }

    [Fact]
    public async Task DeleteAsync_ConActividadExistente_LaEliminaYConfirmaLosCambios()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var activity = new ProjectActivity(projectId, "Actividad a borrar", 1000m, 50m, 40m, 400m);
        SetupExistingActivity(activity);

        // Act
        await _sut.DeleteAsync(projectId, activity.Id, CancellationToken.None);

        // Assert
        _projectActivityRepositoryMock.Verify(repo => repo.Remove(activity), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConActividadInexistente_LanzaEntityNotFoundException()
    {
        // Arrange
        _projectActivityRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectActivity?)null);

        // Act
        var act = () => _sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_CuandoActividadNoPerteneceAlProyecto_LanzaDomainValidationException()
    {
        // Arrange
        var proyectoRealId = Guid.NewGuid();
        var proyectoIncorrectoId = Guid.NewGuid();

        var activity = new ProjectActivity(proyectoRealId, "Actividad", 1000m, 50m, 40m, 400m);
        SetupExistingActivity(activity);

        // Act
        var act = () => _sut.DeleteAsync(proyectoIncorrectoId, activity.Id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("La actividad no pertenece al proyecto indicado.");
    }
}
