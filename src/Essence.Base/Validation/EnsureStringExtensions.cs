using Microsoft;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Essence.Base.Validation;

public static class EnsureStringExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static string IsNotNullOrEmpty(
      this IEnsureClause _,
      [ValidatedNotNull] string? value,
      [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        var validated = Ensure.That.IsNotNull(value, parameterName);

        if (validated == string.Empty)
        {
            throw new ArgumentException($"Parameter '{parameterName}' value is empty.", parameterName);
        }

        return validated;
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static string IsNotNullOrWhitespace(
      this IEnsureClause _,
      [ValidatedNotNull] string? value,
      [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        var validated = Ensure.That.IsNotNull(value, parameterName);

        if (string.IsNullOrWhiteSpace(validated))
        {
            throw new ArgumentException($"Parameter '{parameterName}' value is empty or contains only whitespace.", parameterName);
        }

        return validated;
    }
}