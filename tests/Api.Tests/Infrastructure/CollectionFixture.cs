namespace Api.Tests.Infrastructure;

[CollectionDefinition(CollectionDefinitionName)]
public class CollectionFixture : ICollectionFixture<DataContainerFixture>
{
    public const string CollectionDefinitionName = "Tests Collection";
}