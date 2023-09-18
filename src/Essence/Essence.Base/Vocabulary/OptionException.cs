using System;

namespace Essence.Base.Vocabulary;

public sealed class OptionException : Exception
{
    public OptionException(string message) : base(message) { }
}