using HippocampusSql.Enums;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class SelectDefinition : ISelectDefinition
    {
        public ISqlQuery Query { get; }

        public SelectDefinition(ISqlQuery query)
        {
            Query = query;
            Query.AppendInto(AppendType.Select, s => s.Append("SELECT "));
        }

        public void Dispose()
        {
            Query.AppendInto(AppendType.Select,
                s =>
                {
                    s.AppendLine().Append(" FROM ");
                    var info = Query.ClassCache.TableInfo;

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
