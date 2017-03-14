using System.Collections.Generic;

namespace HippocampusSql.Interfaces
{
    public interface ISqlStatement
    {
        ICollection<ISqlDefinition> Definitions { get; }

        IWhereCommandDefinition WhereCommand { get; set; }

        IClassMetadataCache ClassCache { get; }

        IDictionary<string, object> Parameters { get; }

        string GenerateNewParameter(object value);
    }
}
