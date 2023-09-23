using Essence.Persistence.Postgre.Repositories;
using System;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Unit.Repositories;

public class CookbookRepositoryTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new CookbookRepository(null!));
    }
}
