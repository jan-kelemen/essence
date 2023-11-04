using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration;

[CollectionDefinition(nameof(PostgreCollection))]
public class PostgreCollection : ICollectionFixture<PostgreFixture>
{
}
