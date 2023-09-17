using Essence.Base.Vocabulary;
using Xunit;

namespace Essence.Base.Test.Unit.Vocabulary;

public class ResultTests
{
    [Fact]
    public void ConstructedFromSuccessValue_ContainsSuccessValue()
    {
        var result = new Result<string, int>("42");
        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect);

        Assert.False(result.IsError);
        Assert.Throws<ResultException>(() => result.ExpectError);
    }

    [Fact]
    public void ConstructedFromErrorValue_ContainsErrorValue()
    {
        var result = new Result<int, string>("42");
        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError);

        Assert.False(result.IsSuccess);
        Assert.Throws<ResultException>(() => result.Expect);
    }

    [Fact]
    public void ImplicitlyConstructed_FromSuccessValue()
    {
        static Result<string, int> FunctionWithImplicitConversion()
        {
            return "42";
        }

        var result = FunctionWithImplicitConversion();

        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect);
    }

    [Fact]
    public void ImplicitlyConstructed_FromErrorValue()
    {
        static Result<int, string> FunctionWithImplicitConversion()
        {
            return "42";
        }

        var result = FunctionWithImplicitConversion();

        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError);
    }

    [Fact]
    public void ConstructedWithSuccessDiscriminator_ContainsSuccessValue()
    {
        var result = new Result<string, string>(new SuccessDiscriminator(), "42");

        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect);
    }

    [Fact]
    public void ConstructedWithErrorDiscriminator_ContainsErrorValue()
    {
        var result = new Result<string, string>(new ErrorDiscriminator(), "42");

        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError);
    }
}
