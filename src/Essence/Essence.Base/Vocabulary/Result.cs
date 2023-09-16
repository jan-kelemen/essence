namespace Essence.Base.Vocabulary;

public abstract record Result<T, E>
{
    public static Result<T, E> FromSuccess(T value) => new Success(value);
    public static Result<T, E> FromError(E value) => new Error(value);

    public sealed record Success : Result<T, E>
    {
        private readonly T _value;

        internal Success(T value) { _value = value; }

        public T Value => _value;

        public override bool IsSuccess => true;

        public override bool IsError => false;

        public override T Expect => _value;

        public override E ExpectError =>
            throw new ResultException("Result does not contain a error variant");
    }

    public sealed record Error : Result<T, E>
    {
        private readonly E _value;

        internal Error(E value) { _value = value; }

        public E Value => _value;

        public override bool IsSuccess => false;

        public override bool IsError => true;

        public override T Expect =>
            throw new ResultException("Result does not contain a success variant");

        public override E ExpectError => _value;
    }

    private Result() { }

    public abstract bool IsSuccess { get; }

    public abstract bool IsError { get; }

    public abstract T Expect { get; }

    public abstract E ExpectError { get; }
}
