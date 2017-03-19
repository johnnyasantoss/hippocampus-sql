using System;
using System.Text;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class WhereDefinition : SqlDefinition, IWhereDefinition
    {
        public WhereDefinition(ISqlStatment statementInfo)
        : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            if (Statement.WhereCommand == null)
                Statement.WhereCommand = new WhereCommandDefinition();


            strBuilder.Append(" WHERE FUNC ");
        }
    }
}