using System;
using Essence.Domain.Services.Internal;
using Xunit;

namespace Essence.Domain.Test.Unit.Services.Internal;

public class RecipeServiceTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeService(null!));
    }
}
