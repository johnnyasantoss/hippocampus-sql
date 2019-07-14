using System;

namespace Hippocampus.SQL.Interfaces
{
    internal interface IWhereDefinition : IDisposable
    {
        ISqlQuery Query { get; }
    }
}
