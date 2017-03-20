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
            ISqlStatement info
            , Expression<Func<object[]>> columnsSelector
            , Type selectedType
            )
            : base(info)
        {
            SelectedType = selectedType.CheckNull(nameof(selectedType));
            ExpressionResolver.ResolveSelect(this, columnsSelector);
        }

        public Type SelectedType { get; }

        public override StringBuilder AppendSqlInto(StringBuilder s)
        {
            var info = Statement.ClassCache[SelectedType].TableInfo;

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
