using HippocampusSql.Interfaces;
using System;
using System.Collections.Generic;

namespace HippocampusSql.Definitions
{
    internal class SqlStatement : ISqlStatement
    {
        public ICollection<ISqlDefinition> Definitions { get; } = (ICollection<ISqlDefinition>)new List<ISelectDefinition>();

        public IClassMetadataCache ClassCache { get; } = new ClassMetadataCache();

        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        private volatile int _paramCount = 0;

        public string GenerateNewParameter(object value)
        {
            var key = "@p" + _paramCount;
            Parameters[key] = value;
            _paramCount++;
            return key;
        }
    }
}
