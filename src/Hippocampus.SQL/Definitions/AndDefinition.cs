using System.Text;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class AndDefinition : SqlDefinition, IAndDefinition
    {
        public AndDefinition(ISqlStatement statementInfo)
            : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            return strBuilder.Append(" AND ");
        }
    }
}
