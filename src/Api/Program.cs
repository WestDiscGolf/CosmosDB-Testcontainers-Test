using System.Net;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// talking to the proper emulator, not the integration tests one
builder.Services.AddSingleton<CosmosClient>(sp => new CosmosClient("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/api", async ([FromBody] Person person, CosmosClient cosmosClient) =>
{
    var container = cosmosClient.GetContainer("Database", "data2");

    person.Id = Guid.NewGuid().ToString();

    ItemResponse<Person> response =
        await container.CreateItemAsync(person, new PartitionKey(person.Id), new ItemRequestOptions() { EnableContentResponseOnWrite = true })
            .ConfigureAwait(false);

    return Results.Ok(response.Resource);
});

app.MapGet("/api/{id}", async (string id, CosmosClient cosmosClient) =>
{
    var container = cosmosClient.GetContainer("Database", "data2");

    try
    {
        ItemResponse<Person> itemResponse =
            await container.ReadItemAsync<Person>(
                    id,
                    new PartitionKey(id))
                .ConfigureAwait(false);

        return Results.Ok(itemResponse.Resource);
    }
    catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
    {
        return Results.NotFound();
    }
});

app.MapDelete("/api/{id}", async (string id, CosmosClient cosmosClient) =>
{
    var container = cosmosClient.GetContainer("Database", "data2");

    _ = await container.DeleteItemAsync<Person>(
            id,
            new PartitionKey(id))
        .ConfigureAwait(false);

    return Results.NoContent();
});

app.MapControllers();

app.Run();


/// <summary>
/// Marker interface for reference by the WebApplicationFactory.
/// </summary>
public interface IApiMarker { }

partial class Program : IApiMarker { }