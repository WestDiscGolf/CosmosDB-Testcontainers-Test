using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Api.Tests.Infrastructure;

public class CustomApiFactory : WebApplicationFactory<IApiMarker>
{
    //private readonly string connString = "AccountEndpoint=https://localhost:{0}/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    private readonly string accountEndpoint = "https://localhost:{0}/";
    private readonly string accountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    private readonly int _portNumber = 0;

    public CustomApiFactory(int portNumber)
    {
        _portNumber = portNumber;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //var mappedPort = _dbContainer.GetMappedPublicPort(8081);
            var updated = string.Format(accountEndpoint, _portNumber);

            services.RemoveAll(typeof(CosmosClient));
            services.AddSingleton(sp =>
                {
                    var cosmosClientBuilder = new CosmosClientBuilder(updated, accountKey);
                    cosmosClientBuilder.WithHttpClientFactory(() =>
                    {
                        HttpMessageHandler httpMessageHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                        return new HttpClient(new FixRequestLocationHandler(_portNumber, httpMessageHandler));
                    });
                    cosmosClientBuilder.WithConnectionModeGateway();

                    return cosmosClientBuilder.Build();
                }
            );
        });
    }
}