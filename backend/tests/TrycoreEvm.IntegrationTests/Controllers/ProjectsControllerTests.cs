using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrycoreEvm.Application.Dtos;
using Xunit;

namespace TrycoreEvm.IntegrationTests.Controllers;

public class ProjectsControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();
    private readonly HttpClient _client;

    public ProjectsControllerTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_ConNombreValido_CreaElProyectoYDevuelve201ConLaUbicacionDelRecurso()
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Implementación ERP"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Implementación ERP");
        created.Activities.Should().BeEmpty();
        created.ConsolidatedIndicators.CostPerformanceIndex.Should().BeNull();
    }

    [Fact]
    public async Task Post_ConNombreVacio_Devuelve400ConDetalleDelError()
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest(string.Empty));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_DevuelveTodosLosProyectosCreados()
    {
        await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Proyecto A"));
        await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Proyecto B"));

        var response = await _client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectSummaryResponse>>();
        projects.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ConProyectoExistente_Devuelve200ConElDetalle()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Proyecto A"));
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDetailResponse>();

        var response = await _client.GetAsync($"/api/projects/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var project = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        project!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_ConProyectoInexistente_Devuelve404()
    {
        var response = await _client.GetAsync($"/api/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ConProyectoExistente_RenombraYDevuelve200()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Nombre original"));
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDetailResponse>();

        var response = await _client.PutAsJsonAsync($"/api/projects/{created!.Id}", new ProjectRequest("Nombre actualizado"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        updated!.Name.Should().Be("Nombre actualizado");
    }

    [Fact]
    public async Task Put_ConProyectoInexistente_Devuelve404()
    {
        var response = await _client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}", new ProjectRequest("Nombre"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ConProyectoExistente_Devuelve204YLoEliminaDelListado()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest("Proyecto a eliminar"));
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDetailResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/projects/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/projects/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ConProyectoInexistente_Devuelve404()
    {
        var response = await _client.DeleteAsync($"/api/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
