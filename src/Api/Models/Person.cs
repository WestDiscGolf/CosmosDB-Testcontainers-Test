namespace Api.Models;

public class Person
{
    [Newtonsoft.Json.JsonProperty("id")]
    public string? Id { get; set; }

    [Newtonsoft.Json.JsonProperty("firstName")]
    public string? FirstName { get; set; }

    [Newtonsoft.Json.JsonProperty("lastName")]
    public string? LastName { get; set; }
}