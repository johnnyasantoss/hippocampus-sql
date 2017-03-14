using System.Collections.Generic;

namespace HippocampusSql.Models
{
    /// <summary>
    /// Information of generated SQL Query
    /// </summary>
    public class SqlQuery
    {
        internal SqlQuery()
        {
        }

        /// <summary>
        /// Text content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Named parameters
        /// </summary>
        public IDictionary<string, object> NamedParameters { get; set; }

        /// <summary />
        public static implicit operator string(SqlQuery q)
            => q.Content;
    }
}