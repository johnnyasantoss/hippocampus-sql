using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using HippocampusSql.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql
{
    internal class SqlQuery : ISqlQuery
    {
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public QueryType Type { get; }

        public StringBuilder Where { get; } = new StringBuilder();

        public TableInfo TableInfo { get; set; }

        public SqlQuery(QueryType type)
        {
            Type = type;
        }

        public string GenerateNewParameter(object value)
        {
            var key = "@p" + (Parameters.Count + 1);
            Parameters.Add(key, value);
            return key;
        }

        public string ToSqlString()
        {
            switch (Type)
            {
                case QueryType.Select:
                    return GenerateSelect();
                default:
                    throw new NotImplementedException($"QueryType \"{Type}\" not implemented.");
            }
        }

        private string GenerateSelect()
        {
            var sb = new StringBuilder("SELECT * FROM ");

            sb.Append(TableInfo.Name)
                .Append(" AS ")
                .Append(TableInfo.Abrv)
                .AppendLine(" ");

            if (Where != null)
                sb.Append("WHERE ")
                    .Append(Where.ToString())
                    .AppendLine();

            return sb.ToString();
        }
    }
}