using HippocampusSql.Interfaces;
using HippocampusSql.Services;
using HippocampusSql.Utils;
using System;
using System.Linq.Expressions;
using System.Text;

namespace HippocampusSql.Definitions
{
    public class SelectDefinition : ISelectDefinition
    {
        public SelectDefinition(
            ISqlStatmentInfo info
            , Expression<Func<object[]>> columnsSelector
            )
        {
            info.CheckArgumentNull(nameof(info));

            Info = info;
            ExpressionResolver.ResolveSelect(columnsSelector);
        }

        public ISqlStatmentInfo Info { get; }

        public StringBuilder AppendSqlInto(StringBuilder s)
        {
            s.Append(" FROM ");
            var info = Info.ClassCache.TableInfo;

            if (!string.IsNullOrWhiteSpace(info.Schema))
                s.Append(info.Schema)
                 .Append('.');

            s.Append(info.Name);

            if (!string.IsNullOrWhiteSpace(info.Abbreviation))
                s.Append(" AS ")
                 .Append(info.Abbreviation);

            return s;
        }

        public string ToSql()
            => AppendSqlInto(new StringBuilder()).ToString();
    }
}
