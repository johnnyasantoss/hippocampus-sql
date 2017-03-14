using HippocampusSql.Enums;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class WhereDefinition : IWhereDefinition
    {
        public ISqlStatmentInfo QueryInfo { get; }

        public WhereDefinition(ISqlStatmentInfo sqlQueryInfo)
        {
            QueryInfo = sqlQueryInfo;
            QueryInfo.WhereDefinitions++;

            if (QueryInfo.WhereDefinitions > 1)
                QueryInfo.AppendInto(AppendType.Where,
                    s => s.AppendLine()
                          .Append(" AND (")
                          );
            else
                QueryInfo.AppendInto(AppendType.Where, s => s.Append("WHERE ("));
        }

        public void Dispose()
        {
            QueryInfo.AppendInto(AppendType.Where, s => s.Append(')'));
        }
    }
}