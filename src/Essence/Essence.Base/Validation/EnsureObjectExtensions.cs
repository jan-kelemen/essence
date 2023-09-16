using Microsoft;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Essence.Base.Validation;

public static class EnsureObjectExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static T IsNotNull<T>(
      this IEnsureClause _,
      [ValidatedNotNull] T? value,
      [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null) { throw new ArgumentNullException(parameterName); }

        return value;
    }
}
