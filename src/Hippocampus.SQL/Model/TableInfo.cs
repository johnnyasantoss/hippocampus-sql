namespace HippocampusSql.Model
{
    /// <summary>
    /// Table information
    /// </summary>
    public struct TableInformation
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Abbreviation in sql query (must be unique)
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Schema of the table
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Information of the table
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="schema">Schema of the table</param>
        /// <param name="abbreviation">Abbreviation in sql query (must be unique)</param>
        public TableInformation(string name, string schema = null, string abbreviation = null)
        {
            Abbreviation = abbreviation;
            Name = name;
            Schema = schema;
        }
    }
}