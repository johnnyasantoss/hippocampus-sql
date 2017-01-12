using System.Collections.Generic;

namespace HippocampusSql.Interfaces
{
    /// <summary>
    /// Class cache
    /// </summary>
    public interface IClassMetadataCache
    {
        /// <summary>
        /// Table keys
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Table columns
        /// </summary>
        IEnumerable<string> Columns { get; }
    }
}
