using HippocampusSql.Enums;
using System.Collections.Generic;
using System.Text;
using HippocampusSql.Model;

namespace HippocampusSql.Interfaces
{
    internal interface ISqlQuery
    {
        QueryType Type { get; }

        StringBuilder Where { get; }

        IDictionary<string, object> Parameters { get; }

        TableInfo TableInfo { get; set; }

        string GenerateNewParameter(object value);

        string ToSqlString();
    }
}