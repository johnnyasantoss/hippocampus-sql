using HippocampusSql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql.Definitions
{
    internal class SqlStatement : ISqlStatement
    {
        public ICollection<ISqlDefinition> Definitions { get; } = new List<ISelectDefinition>();
    }
}
