using System;

namespace Hippocampus.SQL.Interfaces
{
    internal interface ISelectDefinition : IDisposable
    {
        ISqlQuery Query { get; }
    }
}