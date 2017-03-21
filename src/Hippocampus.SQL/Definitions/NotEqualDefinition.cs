using HippocampusSql.Interfaces;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class NotEqualDefinition : SqlDefinition, INotEqualDefinition
    {
        public NotEqualDefinition(ISqlStatement statementInfo)
            : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            return strBuilder.Append(" <> ");
        }
    }
}
