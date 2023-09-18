using Essence.Base.Vocabulary;
using System;
using Xunit;

namespace Essence.Base.Test.Unit.Vocabulary;

public class ResultTests
{
    [Fact]
    public void ConstructedFromSuccessValue_ContainsSuccessValue()
    {
        var result = new Result<string, int>("42");
        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect());

        Assert.False(result.IsError);
        Assert.Throws<ResultException>(() => result.ExpectError());
    }

    [Fact]
    public void ConstructedFromErrorValue_ContainsErrorValue()
    {
        var result = new Result<int, string>("42");
        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError());

        Assert.False(result.IsSuccess);
        Assert.Throws<ResultException>(() => result.Expect());
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
        Assert.Equal("42", result.Expect());
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
        Assert.Equal("42", result.ExpectError());
    }

    [Fact]
    public void ConstructedWithSuccessDiscriminator_ContainsSuccessValue()
    {
        var result = new Result<string, string>(new SuccessDiscriminator(), "42");

        Assert.True(result.IsSuccess);
        Assert.Equal("42", result.Expect());
    }

    [Fact]
    public void ConstructedWithErrorDiscriminator_ContainsErrorValue()
    {
        var result = new Result<string, string>(new ErrorDiscriminator(), "42");

        Assert.True(result.IsError);
        Assert.Equal("42", result.ExpectError());
    }

    [Fact]
    public void Map_ThrowsException_WhenPassedNullAsMappingFunction()
    {
        var result = new Result<int, double>(1);

        Func<int, Void>? func = null;
        Assert.Throws<ArgumentNullException>(() => result.Map(func!));
    }

    [Fact]
    public void Map_ConvertsSuccessValue_ToNewType()
    {
        var result = new Result<int, double>(1);

        var mapped = result.Map(t => t.ToString());
        Assert.True(mapped.IsSuccess);
        Assert.Equal("1", mapped.Expect());
    }

    [Fact]
    public void Map_LeavesErrorValue_Unchanged_WhenContainingError()
    {
        var result = new Result<int, double>(Math.PI);

        var mapped = result.Map(t => t.ToString());
        Assert.True(mapped.IsError);
        Assert.Equal(Math.PI, mapped.ExpectError());
    }

    [Fact]
    public void MapOr_ThrowsException_WhenPassedNullAsMappingFunction()
    {
        var result = new Result<int, double>(1);

        Func<int, Void>? func = null;
        Assert.Throws<ArgumentNullException>(() => result.MapOr(func!, Void.Value));
    }

    [Fact]
    public void MapOr_ConvertsSuccessValue_ToNewType()
    {
        var result = new Result<int, double>(1);

        var mapped = result.MapOr(t => t.ToString(), Math.PI.ToString());
        Assert.Equal("1", mapped);
    }

    [Fact]
    public void MapOr_ReturnsDefaultValue_WhenContainingError()
    {
        var result = new Result<int, double>(Math.PI);

        var mapped = result.MapOr(t => t.ToString(), "1");
        Assert.Equal("1", mapped);
    }

    [Fact]
    public void MapError_ThrowsException_WhenPassedNullAsMappingFunction()
    {
        var result = new Result<int, double>(1);

        Func<double, Void>? func = null;
        Assert.Throws<ArgumentNullException>(() => result.MapError(func!));
    }

    [Fact]
    public void MapError_ConvertsErrorValue_ToNewType()
    {
        var result = new Result<int, double>(Math.PI);

        var mapped = result.MapError(t => t.ToString());
        Assert.True(mapped.IsError);
        Assert.Equal(Math.PI.ToString(), mapped.ExpectError());
    }

    [Fact]
    public void MapError_LeavesSuccessValueUnchanged_WhenContainingSuccess()
    {
        var result = new Result<int, double>(1);

        var mapped = result.MapError(t => t.ToString());
        Assert.True(mapped.IsSuccess);
        Assert.Equal(1, mapped.Expect());
    }

    [Fact]
    public void MapOrElse_ThrowsException_WhenPassedNullAsMappingFunction()
    {
        var result = new Result<int, double>(1);

        Func<int, Void>? mapSucc = null;
        Func<double, Void>? mapErr = null;
        Assert.Throws<ArgumentNullException>(() => result.MapOrElse(mapSucc!, u => Void.Value));
        Assert.Throws<ArgumentNullException>(() => result.MapOrElse(t => Void.Value, mapErr!));
    }

    [Fact]
    public void MapOrElse_MapsContainedValues()
    {
        var successResult = new Result<int, double>(1);
        var errorResult = new Result<int, double>(Math.PI);

        Func<int, string> mapSuccess = t => "success";
        Func<double, string> mapError = t => "error";

        Assert.Equal("success", successResult.MapOrElse(mapSuccess, mapError));
        Assert.Equal("error", errorResult.MapOrElse(mapSuccess, mapError));
    }
}
