using HippocampusSql.Interfaces;
using HippocampusSql.Utils;
using System;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class SelectItemDefinition : SqlDefinition, ISelectItem
    {
        public string Expression { get; }

        public string Alias { get; }

        public SelectItemDefinition(
            ISqlStatement statement
            , string expression
            , string alias = null
            ) : base(statement)
        {
            Expression = expression.CheckNull(nameof(expression));
            Alias = alias.CheckEmpty(nameof(alias));
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
