using System;
using Essence.Domain.Model;
using Xunit;

namespace Essence.Domain.Test.Unit.Model;

public class IngredientTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Name_IsNeverEmpty(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new IngredientHeader(new("ID"), name!));
    }
}
