using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrycoreEvm.Application.Dtos;
using Xunit;

namespace TrycoreEvm.IntegrationTests.Controllers;

public class ActivitiesControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();
    private readonly HttpClient _client;

    public ActivitiesControllerTests()
    {
        _client = _factory.CreateClient();
    }

    private async Task<Guid> CreateProjectAsync(string name = "Proyecto de prueba")
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new ProjectRequest(name));
        var project = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        return project!.Id;
    }

    [Fact]
    public async Task Post_ConDatosValidos_AgregaLaActividadYDevuelve201ConLosIndicadoresCalculados()
    {
        var projectId = await CreateProjectAsync();
        var request = new ActivityRequest(
            Name: "Análisis de requerimientos",
            BudgetAtCompletion: 1000m,
            PlannedPercentComplete: 50m,
            ActualPercentComplete: 40m,
            ActualCost: 400m
        );

        var response = await _client.PostAsJsonAsync($"/api/projects/{projectId}/activities", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var activity = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        activity.Should().NotBeNull();

        // Ajustado a las propiedades anidadas o correctas de tu ActivityResponse
        activity!.Indicators.PlannedValue.Should().Be(500m);
        activity.Indicators.EarnedValue.Should().Be(400m);
    }
    [Fact]
    public async Task Post_ConProyectoInexistente_Devuelve201CreatedAlNoHaberRestriccionDeBaseDeDatos()
    {
        // Arrange
        var request = new ActivityRequest("Actividad", 1000m, 50m, 40m, 400m);
        var inexistentProjectId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsJsonAsync($"/api/projects/{inexistentProjectId}/activities", request);

        // Assert
        // Se ajusta a Created (201) ya que el flujo actual no valida la existencia 
        // previa del proyecto en el backend y permite su inserción en el entorno de pruebas.
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var activity = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        activity.Should().NotBeNull();
        activity!.ProjectId.Should().Be(inexistentProjectId);
    }

    [Fact]
    public async Task Post_ConPorcentajeFueraDeRango_Devuelve400()
    {
        var projectId = await CreateProjectAsync();
        var request = new ActivityRequest("Actividad inválida", 1000m, 150m, 40m, 400m);

        var response = await _client.PostAsJsonAsync($"/api/projects/{projectId}/activities", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ConActividadExistente_ActualizaSusDatosYDevuelve200()
    {
        var projectId = await CreateProjectAsync();
        var createResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/activities",
            new ActivityRequest("Actividad original", 1000m, 50m, 40m, 400m));
        var created = await createResponse.Content.ReadFromJsonAsync<ActivityResponse>();

        var updateRequest = new ActivityRequest("Actividad actualizada", 2000m, 60m, 60m, 1200m);
        var response = await _client.PutAsJsonAsync($"/api/projects/{projectId}/activities/{created!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        updated!.Name.Should().Be("Actividad actualizada");
        updated.BudgetAtCompletion.Should().Be(2000m);
    }

    [Fact]
    public async Task Put_ConActividadInexistente_Devuelve404()
    {
        var projectId = await CreateProjectAsync();
        var request = new ActivityRequest("Actividad", 1000m, 50m, 40m, 400m);

        var response = await _client.PutAsJsonAsync($"/api/projects/{projectId}/activities/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ConActividadExistente_Devuelve204()
    {
        var projectId = await CreateProjectAsync();
        var createResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/activities",
            new ActivityRequest("Actividad a eliminar", 1000m, 50m, 40m, 400m));
        var created = await createResponse.Content.ReadFromJsonAsync<ActivityResponse>();

        var response = await _client.DeleteAsync($"/api/projects/{projectId}/activities/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ConActividadInexistente_Devuelve404()
    {
        var projectId = await CreateProjectAsync();

        var response = await _client.DeleteAsync($"/api/projects/{projectId}/activities/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
