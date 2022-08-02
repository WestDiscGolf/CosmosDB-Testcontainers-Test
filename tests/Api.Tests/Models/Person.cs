using System.Text.Json.Serialization;

namespace Api.Tests.Models;

public class Person
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
}