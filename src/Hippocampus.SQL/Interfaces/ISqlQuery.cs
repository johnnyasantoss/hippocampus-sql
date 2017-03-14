using System.Collections.Generic;

namespace HippocampusSql.Interfaces
{
    interface ISqlQuery
    {
        IEnumerable<ISqlStatement> Statments { get; }
    }
}
