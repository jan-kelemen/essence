using Essence.Base.Validation;
using System;
using Xunit;

namespace Essence.Base.Test.Unit.Validation;

public class EnsureObjectExtensionsTests
{
    [Fact]
    public void IsNotNull_ThrowsArgumentNullException_GivenNullParameter()
    {
        object? obj = null;
        var exception = Assert.Throws<ArgumentNullException>(() => Ensure.That.IsNotNull(obj));
        Assert.Equal(nameof(obj), exception.ParamName);
    }

    [Fact]
    public void IsNotNull_DoesNotThrow_GivenNonNullParameter()
    {
        var returnedValue = Ensure.That.IsNotNull(nameof(IsNotNull_DoesNotThrow_GivenNonNullParameter));
        Assert.Equal(nameof(IsNotNull_DoesNotThrow_GivenNonNullParameter), returnedValue);
    }
}