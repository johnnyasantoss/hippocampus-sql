using System;

namespace HippocampusSql.Interfaces
{
    public interface ISelectDefinition : ISqlDefinition
    {
        Type SelectedType { get; }
    }
}
