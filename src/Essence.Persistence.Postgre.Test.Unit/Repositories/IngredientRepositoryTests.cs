using System;
using Essence.Persistence.Postgre.Repositories;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Unit.Repositories;

public class IngredientRepositoryTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new IngredientRepository(null!));
    }
}
