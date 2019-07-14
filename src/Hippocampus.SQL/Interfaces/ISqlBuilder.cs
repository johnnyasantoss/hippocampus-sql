using System;
using System.Linq.Expressions;
using Hippocampus.SQL.Model;

namespace Hippocampus.SQL.Interfaces
{
    /// <summary>
    /// SQL builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public interface ISqlBuilder<T> where T : class
    {
        /// <summary>
        /// Defines if query should have line-breaks
        /// </summary>
        bool Beautify { get; set; }

        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Select(TableInformation? info = null);

        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="selector">Expression to return a object array of the columns(props) that you want to select</param>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Select(Expression<Func<T, object[]>> selector, TableInformation? info = null);

        /// <summary>
        /// Generates a insert query
        /// </summary>
        /// <param name="entity">Entity to be inserted</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Insert(T entity);

        /// <summary>
        /// Generates a update query
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Update(T entity);

        /// <summary>
        /// Generates a delete query
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Delete(T entity);

        /// <summary>
        /// Generates a where over this query
        /// </summary>
        /// <param name="predicate">A predicate to iterate over the query</param>
        /// <returns>This builder</returns>
        ISqlBuilder<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Generates the sql string made out of this builder
        /// </summary>
        /// <returns>SQL string</returns>
        string Materialize();
    }
}
