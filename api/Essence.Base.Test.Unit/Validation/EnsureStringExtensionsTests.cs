using Essence.Base.Validation;
using System;
using Xunit;

namespace Essence.Base.Test.Unit.Validation;

public class EnsureStringExtensionsTests
{
    [Fact]
    public void IsNotNullOrEmpty_ThrowsArgumentNullException_GivenNullParameter()
    {
        string? str = null;
        var exception = Assert.Throws<ArgumentNullException>(() => Ensure.That.IsNotNullOrEmpty(str));
        Assert.Equal(nameof(str), exception.ParamName);
    }

    [Fact]
    public void IsNotNullOrEmpty_ThrowsArgumentException_GivenEmptyString()
    {
        string str = string.Empty;
        var exception = Assert.Throws<ArgumentException>(() => Ensure.That.IsNotNullOrEmpty(str));
        Assert.Equal(nameof(str), exception.ParamName);
    }

    [Fact]
    public void IsNotNullOrEmpty_DoesNotThrow_GivenValidString()
    {
        string? str = nameof(IsNotNullOrEmpty_DoesNotThrow_GivenValidString);
        var returnedValue = Ensure.That.IsNotNullOrEmpty(str);
        Assert.Equal(nameof(IsNotNullOrEmpty_DoesNotThrow_GivenValidString), returnedValue);
    }

    [Fact]
    public void IsNotNullOrWhitespace_ThrowsArgumentNullException_GivenNullParameter()
    {
        string? str = null;
        var exception = Assert.Throws<ArgumentNullException>(() => Ensure.That.IsNotNullOrWhitespace(str));
        Assert.Equal(nameof(str), exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void IsNotNullOrWhitespace_ThrowsArgumentException_GivenEmptyOrWhitespaceString(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => Ensure.That.IsNotNullOrWhitespace(value));
        Assert.Equal(nameof(value), exception.ParamName);
    }

    [Fact]
    public void IsNotNullOrWhitespace_DoesNotThrow_GivenValidString()
    {
        string? str = nameof(IsNotNullOrEmpty_DoesNotThrow_GivenValidString);
        var returnedValue = Ensure.That.IsNotNullOrEmpty(str);
        Assert.Equal(nameof(IsNotNullOrEmpty_DoesNotThrow_GivenValidString), returnedValue);
    }
}