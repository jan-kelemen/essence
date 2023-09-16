using Essence.Base.Vocabulary;
using Xunit;

namespace Essence.Base.Test.Unit.Vocabulary;

public class OptionTests
{
    [Fact]
    public void DefaultConstructedOption_HasNoValue()
    {
        var option = new Option<int>();
        Assert.False(option.HasValue);
        Assert.Throws<OptionException>(() => option.Value);
    }

    [Fact]
    public void OptionConstructedFromValue_HasValue()
    {
        var option = new Option<int>(42);
        Assert.True(option.HasValue);
        Assert.Equal(42, option.Value);
    }
}
