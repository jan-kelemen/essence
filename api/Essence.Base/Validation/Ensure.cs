namespace Essence.Base.Validation;

public interface IEnsureClause { }

public sealed class Ensure : IEnsureClause
{
    public static readonly Ensure That = new();

    private Ensure() { }
}
