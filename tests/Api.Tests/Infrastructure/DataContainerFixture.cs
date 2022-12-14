using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Azure.Cosmos.Fluent;

namespace Api.Tests.Infrastructure;

public class DataContainerFixture : IAsyncLifetime
{
    private readonly ITestcontainersContainer _dbContainer;
    private readonly IOutputConsumer consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());

    //private readonly string connString = "AccountEndpoint=https://localhost:{0}/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    private readonly string accountEndpoint = "https://localhost:{0}/";
    private readonly string accountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    public int GetMappedPort() => _dbContainer.GetMappedPublicPort(8081);

    public DataContainerFixture()
    {
        _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
            .WithExposedPort(8081)
            .WithExposedPort(10251)
            .WithExposedPort(10252)
            .WithExposedPort(10253)
            .WithExposedPort(10254)
            .WithPortBinding(8081, true)
            .WithPortBinding(10251, true)
            .WithPortBinding(10252, true)
            .WithPortBinding(10253, true)
            .WithPortBinding(10254, true)
            .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "10")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
            .WithOutputConsumer(consumer)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged(consumer.Stdout, "Started"))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // everything below here is about creating the test database and container
        var mappedPort = _dbContainer.GetMappedPublicPort(8081);
        var updated = string.Format(accountEndpoint, mappedPort);
        var cosmosClientBuilder = new CosmosClientBuilder(updated, accountKey);
        cosmosClientBuilder.WithHttpClientFactory(() =>
        {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(new FixRequestLocationHandler(mappedPort, httpMessageHandler));
        });
        cosmosClientBuilder.WithConnectionModeGateway();

        var cosmosClient = cosmosClientBuilder.Build();

        var response = await cosmosClient.CreateDatabaseAsync("Database", 4000);
        await response.Database.CreateContainerAsync("data2", "/id", 4000);
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}