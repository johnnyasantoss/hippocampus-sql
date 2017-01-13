using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using System;

namespace HippocampusSql
{
    internal class WhereDefinition : IWhereDefinition
    {
        public ISqlQuery Query { get; }

        public WhereDefinition(ISqlQuery sqlQuery)
        {
            Query = sqlQuery;
            Query.WhereDefinitions++;

            if (Query.WhereDefinitions > 1)
                Query.AppendInto(AppendType.Where,
                    s => s.AppendLine()
                          .Append(" AND (")
                          );
            else
                Query.AppendInto(AppendType.Where, s => s.Append('('));
        }

        public void Dispose()
        {
            Query.AppendInto(AppendType.Where, s => s.Append(')'));
        }
    }
}