using HippocampusSql.Interfaces;
using System;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class ColumnDefinition : SqlDefinition, IColumnDefinition
    {
        public ColumnDefinition(
            ISqlStatement statementInfo
            , string name
            , IClassMetadata metadata
            , bool isFirst = false
            , bool isLast = false
            )
            : base(statementInfo)
        {
            IsFirst = isFirst;
            IsLast = isLast;
        }

        public bool IsFirst { get; private set; }

        public bool IsLast { get; private set; }

        public override StringBuilder AppendSqlInto(StringBuilder strBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
