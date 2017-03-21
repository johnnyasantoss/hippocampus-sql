using System;
using System.Text;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class WhereCommandDefinition : SqlDefinition, IWhereCommandDefinition
    {
        public WhereCommandDefinition(ISqlStatement statementInfo)
        : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            throw new NotImplementedException();
        }
    }
}