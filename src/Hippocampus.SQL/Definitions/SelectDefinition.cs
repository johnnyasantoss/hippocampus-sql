using Hippocampus.SQL.Enums;
using Hippocampus.SQL.Interfaces;

namespace Hippocampus.SQL.Definitions
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
                    s.Append(" FROM ");
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
