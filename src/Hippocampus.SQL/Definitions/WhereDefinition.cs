﻿using System.Text;
using HippocampusSql.Interfaces;

namespace HippocampusSql.Definitions
{
    internal class WhereDefinition : SqlDefinition, IWhereDefinition
    {
        public WhereDefinition(ISqlStatement statementInfo)
        : base(statementInfo)
        {
        }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            return strBuilder.Append(" WHERE FUNC ");
        }
    }
}