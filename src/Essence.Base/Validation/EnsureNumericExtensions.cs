using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Essence.Base.Validation;

public static class EnsureNumericExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static T InRange<T>(
      this IEnsureClause _,
      T value,
      T min,
      T max,
      [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : INumber<T>
    {
        if (value < min || value > max) { throw new OverflowException(parameterName); }

        return value;
    }
}
