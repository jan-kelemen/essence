using System;
using Essence.Domain.Vocabulary;

namespace Essence.Persistence.Postgre.Utility;

internal static class PostgreIdentifierExtensions
{
    public static Guid ToPostgreIdentifier(this Identifier id) => Guid.Parse(id.Key);

    public static Identifier ToDomainIdentifier(this Guid id) => new(id.ToString());
}
