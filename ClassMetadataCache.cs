using HippocampusSql.Interfaces;
using System.Collections.Generic;

namespace HippocampusSql
{
    /// <summary>
    /// Stores mapped class info
    /// </summary>
    public class ClassMetadataCache : IClassMetadataCache
    {
        /// <summary>
        /// Mapped columns
        /// </summary>
        public List<string> Columns { get; } = new List<string>();

        /// <summary>
        /// Mapped Keys
        /// </summary>
        public List<string> Keys { get; } = new List<string>();

        IEnumerable<string> IClassMetadataCache.Columns
            => Columns;

        IEnumerable<string> IClassMetadataCache.Keys
            => Keys;
    }
}