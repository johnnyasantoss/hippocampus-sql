using HippocampusSql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HippocampusSql.Definitions
{
    class SqlStatement : ISqlStatement
    {
        public ICollection<ISqlDefinition> Definitions { get; } = (ICollection<ISqlDefinition>)new List<ISelectDefinition>();

        public IWhereCommandDefinition WhereCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
