using System;
using Essence.Domain.Services.Internal;
using Xunit;

namespace Essence.Domain.Test.Unit.Services.Internal;

public class CookbookServiceTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new CookbookService(null!));
    }
}
