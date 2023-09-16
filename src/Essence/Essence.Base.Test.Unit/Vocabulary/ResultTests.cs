using Essence.Base.Vocabulary;
using Xunit;

namespace Essence.Base.Test.Unit.Vocabulary;

public class ResultTests
{
    [Fact]
    public void ResultConstructedFromSuccessValue_ContainsSuccessValue()
    {
        var result = Result<string, string>.FromSuccess("42");
        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect);

        var value = result switch
        {
            Result<string, string>.Success succ => succ.Value,
            Result<string, string>.Error => "Success result should not contain error variant",
            _ => "Really should not happen"
        };
        Assert.Equal("42", value);

        Assert.False(result.IsError);
        Assert.Throws<ResultException>(() => result.ExpectError);
    }

    [Fact]
    public void ResultConstructedFromErrorValue_ContainsErrorValue()
    {
        var result = Result<string, string>.FromError("42");
        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError);

        var value = result switch
        {
            Result<string, string>.Success => "Error result should not contain success variant",
            Result<string, string>.Error err => err.Value,
            _ => "Really should not happen"
        };
        Assert.Equal("42", value);

        Assert.False(result.IsSuccess);
        Assert.Throws<ResultException>(() => result.Expect);
    }
}
