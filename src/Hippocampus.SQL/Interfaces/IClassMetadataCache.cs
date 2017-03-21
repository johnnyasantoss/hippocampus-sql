using System;

namespace HippocampusSql.Interfaces
{
    /// <summary>
    /// Class cache
    /// </summary>
    public interface IClassMetadataCache
    {
        IClassMetadata this[Type classType] { get; }

        bool Contains(Type type);
    }
}
