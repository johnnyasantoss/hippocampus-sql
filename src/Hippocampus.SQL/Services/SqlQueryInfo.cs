using HippocampusSql.Definitions;
using HippocampusSql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql.Services
{
    internal class SqlQueryInfo : ISqlStatmentInfo
    {
        public SqlQueryInfo(IClassMetadataCache classCache, bool beautify)
        {
            _beautify = beautify;
            ClassCache = classCache;
        }

        private readonly bool _beautify;

        public IClassMetadataCache ClassCache { get; }

        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public StringBuilder Where { get; } = new StringBuilder();

        public int WhereDefinitions { get; set; }

        public StringBuilder Select { get; } = new StringBuilder();

        public string GenerateNewParameter(object value)
        {
            var key = "@p" + (Parameters.Count + 1);
            Parameters.Add(key, value);
            return key;
        }

        public string ToSqlString()
        {
            return new StringBuilder(Select.ToString())
                .Append(Where)
                .ToString()
                .TrimEnd();
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
            => new WhereDefinition(this);

        public ISelectDefinition BeginSelect()
            => new SelectDefinition(this);
    }
}