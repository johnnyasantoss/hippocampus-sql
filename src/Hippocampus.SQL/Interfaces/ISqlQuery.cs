using System;
using System.Collections.Generic;
using System.Text;
using Hippocampus.SQL.Enums;

namespace Hippocampus.SQL.Interfaces
{
    internal interface ISqlQuery
    {
        IClassMetadataCache ClassCache { get; }

        StringBuilder Select { get; }

        StringBuilder Where { get; }

        int WhereDefinitions { get; set; }

        IDictionary<string, object> Parameters { get; }

        StringBuilder AppendInto(AppendType type, Func<StringBuilder, StringBuilder> appender);

        string GenerateNewParameter(object value);

        string ToSqlString();

        IWhereDefinition BeginWhere();

        ISelectDefinition BeginSelect();
    }
}