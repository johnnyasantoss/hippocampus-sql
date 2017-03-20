using HippocampusSql.Models;
using System.Collections.Generic;

namespace HippocampusSql.Interfaces
{
    public interface IClassMetadata
    {
        /// <summary>
        /// Table keys
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Table columns
        /// </summary>
        IEnumerable<string> Columns { get; }

        /// <summary>
        /// Information of the table, it schema and it abbreviation
        /// </summary>
        TableInformation TableInfo { get; }
    }
}
