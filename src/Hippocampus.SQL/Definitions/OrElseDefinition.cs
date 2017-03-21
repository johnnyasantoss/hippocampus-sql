using HippocampusSql.Interfaces;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class OrElseDefinition : SqlDefinition, IOrElseDefinition
    {
        public OrElseDefinition(ISqlStatement statementInfo)
            : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            return strBuilder.Append(" OR ");
        }
    }
}
