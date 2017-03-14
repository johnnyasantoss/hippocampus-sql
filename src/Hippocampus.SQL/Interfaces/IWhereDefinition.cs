using System;

namespace HippocampusSql.Interfaces
{
    internal interface IWhereDefinition : IDisposable
    {
        ISqlStatmentInfo QueryInfo { get; }
    }
}
