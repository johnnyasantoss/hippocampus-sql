using System;

namespace HippocampusSql.Interfaces
{
    internal interface IWhereDefinition : IDisposable
    {
        ISqlQueryInfo QueryInfo { get; }
    }
}
