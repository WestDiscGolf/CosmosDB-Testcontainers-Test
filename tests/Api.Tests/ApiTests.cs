using System.Net.Http.Json;
using Api.Tests.Infrastructure;
using Api.Tests.Models;

namespace Api.Tests;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class ApiTests
{
    private readonly DataContainerFixture _dataContainerFixture;

    public ApiTests(DataContainerFixture dataContainerFixture)
    {
        _dataContainerFixture = dataContainerFixture;
    }


    [Fact]
    public async Task Test1()
    {
        // Arrange
        var factory = new CustomApiFactory(_dataContainerFixture.GetMappedPort());
        var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("weatherforecast");

        // Assert
        var content = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();
        content.Should().HaveCount(5);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateAndRead(Person person)
    {
        // Arrange
        var factory = new CustomApiFactory(_dataContainerFixture.GetMappedPort());
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api", person);

        // Assert
        var p = await response.Content.ReadFromJsonAsync<Person>();
        p.FirstName.Should().Be(person.FirstName);
        p.LastName.Should().Be(person.LastName);

        Guid.TryParse(p.Id, out Guid id).Should().BeTrue();

        var response2 = await client.GetFromJsonAsync<Person>($"/api/{id}");
        response2.FirstName.Should().Be(person.FirstName);
        response2.LastName.Should().Be(person.LastName);
        response2.Id.Should().Be(p.Id);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateAndDelete(Person person)
    {
        // Arrange
        var factory = new CustomApiFactory(_dataContainerFixture.GetMappedPort());
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api", person);

        // Assert
        var p = await response.Content.ReadFromJsonAsync<Person>();
        p.FirstName.Should().Be(person.FirstName);
        p.LastName.Should().Be(person.LastName);

        Guid.TryParse(p.Id, out Guid id).Should().BeTrue();

        await client.DeleteAsync($"/api/{id}");

        //var response2 = await client.GetFromJsonAsync<Person>($"/api/{id}");
    }
}