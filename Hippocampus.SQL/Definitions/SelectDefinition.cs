using HippocampusSql.Enums;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class SelectDefinition : ISelectDefinition
    {
        public ISqlQueryInfo QueryInfo { get; }

        public SelectDefinition(ISqlQueryInfo queryInfo)
        {
            QueryInfo = queryInfo;
            QueryInfo.AppendInto(AppendType.Select, s => s.Append("SELECT "));
        }

        public void Dispose()
        {
            QueryInfo.AppendInto(AppendType.Select,
                s =>
                {
                    s.AppendLine().Append(" FROM ");
                    var info = QueryInfo.ClassCache.TableInfo;

                    if (!string.IsNullOrWhiteSpace(info.Schema))
                        s.Append(info.Schema)
                         .Append('.');

                    s.Append(info.Name);

                    if (!string.IsNullOrWhiteSpace(info.Abbreviation))
                        s.Append(" AS ")
                         .Append(info.Abbreviation);

                    return s.AppendLine();
                });
        }
    }
}
