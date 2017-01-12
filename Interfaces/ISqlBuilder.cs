namespace HippocampusSql.Interfaces
{
    /// <summary>
    /// SQL builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public interface ISqlBuilder<T> where T : class
    {
        /// <summary>
        /// Stores info from class
        /// </summary>
        IClassMetadataCache ClassCache { get; }
    }
}
