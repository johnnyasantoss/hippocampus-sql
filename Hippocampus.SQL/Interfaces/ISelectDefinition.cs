using System;

namespace HippocampusSql.Interfaces
{
    internal interface ISelectDefinition : IDisposable
    {
        ISqlQueryInfo QueryInfo { get; }
    }
}