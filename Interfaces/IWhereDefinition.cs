using System;

namespace HippocampusSql.Interfaces
{
    internal interface IWhereDefinition : IDisposable
    {
        ISqlQuery Query { get; }
    }
}
