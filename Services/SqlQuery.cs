using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using HippocampusSql.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql.Services
{
    internal class SqlQuery : ISqlQuery
    {
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public QueryType Type { get; }

        public TableInfo TableInfo { get; set; }

        public StringBuilder Where { get; } = new StringBuilder();

        public int WhereDefinitions { get; set; }

        public StringBuilder Select { get; } = new StringBuilder();

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

        public StringBuilder AppendInto(AppendType type, Func<StringBuilder, StringBuilder> appender)
        {
            StringBuilder sb;

            switch (type)
            {
                case AppendType.Select:
                    sb = Select;
                    break;
                case AppendType.Where:
                    sb = Where;
                    break;
                default:
                    throw new NotImplementedException($"Append type not implemented: {type}");
            }

            return appender(sb);
        }

        public IWhereDefinition BeginWhere()
        {
            return new WhereDefinition(this);
        }
    }
}