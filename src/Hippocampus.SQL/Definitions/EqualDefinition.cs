using HippocampusSql.Interfaces;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class EqualDefinition : SqlDefinition, IEqualDefinition
    {
        public EqualDefinition(ISqlStatement statementInfo)
            : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            return strBuilder.Append(" = ");
        }
    }
}
