using FluentAssertions;
using Moq;
using TrycoreEvm.Application.Dtos;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Application.Services;
using TrycoreEvm.Domain.Entities;
using TrycoreEvm.Domain.Exceptions;
using Xunit;

namespace TrycoreEvm.UnitTests.Application;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _sut = new ProjectService(_projectRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_DevuelveElResumenDeTodosLosProyectos()
    {
        var projects = new List<Project> { new("Proyecto A"), new("Proyecto B") };
        _projectRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().BeEquivalentTo("Proyecto A", "Proyecto B");
    }

    [Fact]
    public async Task GetByIdAsync_ConProyectoExistente_DevuelveElDetalleDelProyecto()
    {
        var project = new Project("Proyecto A");
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.GetByIdAsync(project.Id, CancellationToken.None);

        result.Id.Should().Be(project.Id);
        result.Name.Should().Be("Proyecto A");
    }

    [Fact]
    public async Task GetByIdAsync_ConProyectoInexistente_LanzaEntityNotFoundException()
    {
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var act = () => _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_PersisteElProyectoYConfirmaLosCambios()
    {
        var request = new ProjectRequest("Nuevo proyecto");

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        result.Name.Should().Be("Nuevo proyecto");
        _projectRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RenameAsync_ConProyectoExistente_ActualizaElNombreYConfirmaLosCambios()
    {
        var project = new Project("Nombre original");
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.RenameAsync(project.Id, new ProjectRequest("Nombre actualizado"), CancellationToken.None);

        result.Name.Should().Be("Nombre actualizado");
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConProyectoExistente_EliminaElProyectoYConfirmaLosCambios()
    {
        var project = new Project("Proyecto a eliminar");
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        await _sut.DeleteAsync(project.Id, CancellationToken.None);

        _projectRepositoryMock.Verify(repository => repository.Remove(project), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConProyectoInexistente_LanzaEntityNotFoundExceptionYNoConfirmaCambios()
    {
        _projectRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
