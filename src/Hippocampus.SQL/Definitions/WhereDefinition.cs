using System;
using System.Text;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class WhereDefinition : IWhereDefinition
    {
        public WhereDefinition(ISqlStatmentInfo sqlQueryInfo)
        {
        }

        public ISqlStatement Statement { get; }

        public StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            if (Statement.WhereCommand == null)
                Statement.WhereCommand = new WhereCommandDefinition();


            strBuilder.Append(" WHERE FUNC ");
        }

        public string ToSql()
            => AppendSqlInto(new StringBuilder()).ToString();
    }
}