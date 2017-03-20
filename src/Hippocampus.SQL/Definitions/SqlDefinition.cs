using System.Text;
using HippocampusSql.Interfaces;
using HippocampusSql.Utils;

namespace HippocampusSql.Definitions
{
    internal abstract class SqlDefinition : ISqlDefinition
    {
        public SqlDefinition(ISqlStatement statementInfo)
        {
            statementInfo.CheckNull(nameof(statementInfo));
            Statement = statementInfo;
        }

        public virtual ISqlStatement Statement { get; }

        public abstract StringBuilder AppendSqlInto(StringBuilder strBuilder);

        public virtual string ToSql()
            => AppendSqlInto(new StringBuilder()).ToString();
    }
}