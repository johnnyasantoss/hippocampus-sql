using HippocampusSql.Enums;
using HippocampusSql.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql.Interfaces
{
    internal interface ISqlQuery
    {
        QueryType Type { get; }

        StringBuilder Select { get; }

        StringBuilder Where { get; }

        int WhereDefinitions { get; set; }

        IDictionary<string, object> Parameters { get; }

        TableInfo TableInfo { get; set; }

        StringBuilder AppendInto(AppendType type, Func<StringBuilder, StringBuilder> appender);

        string GenerateNewParameter(object value);

        string ToSqlString();

        IWhereDefinition BeginWhere();
    }
}