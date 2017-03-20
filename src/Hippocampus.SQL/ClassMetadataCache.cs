using System;
using HippocampusSql.Interfaces;
using System.Collections.Generic;

namespace HippocampusSql
{
    /// <summary>
    /// Stores mapped class info
    /// </summary>
    public class ClassMetadataCache : IClassMetadataCache
    {
        private static readonly IDictionary<string, IClassMetadata> _cache = new Dictionary<string, IClassMetadata>();

        private static readonly object _cacheLock = new object();

        public IClassMetadata this[Type classType]
        {
            get
            {
                lock (_cacheLock)
                {
                    var fullName = classType.FullName;

                    if (!_cache.ContainsKey(fullName))
                        _cache[fullName] = GetClassMetadata(classType);

                    return _cache[fullName];
                }
            }
        }

        private IClassMetadata GetClassMetadata(Type classType)
        {
            //TODO: Implement a better log of this action
            return new ClassMetadata(classType);
        }
    }
}