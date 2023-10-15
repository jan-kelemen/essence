using System;
using Essence.Domain.Vocabulary;
using Xunit;

namespace Essence.Domain.Test.Unit.Vocabulary;

public class IdentifierTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Key_IsNeverEmpty(string key)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Identifier(key));
    }
}
