using System.Collections.Generic;

namespace HippocampusSql.Interfaces
{
    public interface ISqlStatmentInfo
    {
        IClassMetadataCache ClassCache { get; }

        IDictionary<string, object> Parameters { get; }

        string GenerateNewParameter(object value);
    }
}