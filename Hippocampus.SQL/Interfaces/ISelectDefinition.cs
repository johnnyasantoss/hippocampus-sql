using System;

namespace HippocampusSql.Interfaces
{
    internal interface ISelectDefinition : IDisposable
    {
        ISqlQuery Query { get; }
    }
}