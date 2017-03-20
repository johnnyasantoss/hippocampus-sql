using HippocampusSql.Interfaces;
using HippocampusSql.Services;
using HippocampusSql.Utils;
using System;
using System.Linq.Expressions;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class SelectDefinition : SqlDefinition, ISelectDefinition
    {
        public SelectDefinition(
            ISqlStatmentInfo info
            , Expression<Func<object[]>> columnsSelector
            )
            : base(info)
        {
            ExpressionResolver.ResolveSelect(columnsSelector);
        }

        public override StringBuilder AppendSqlInto(StringBuilder s)
        {
            s.Append(" FROM ");
            var info = Statment.ClassCache.TableInfo;

            if (!string.IsNullOrWhiteSpace(info.Schema))
                s.Append(info.Schema)
                 .Append('.');

            s.Append(info.Name);

            if (!string.IsNullOrWhiteSpace(info.Abbreviation))
                s.Append(" AS ")
                 .Append(info.Abbreviation);

            return s;
        }
    }
}
