using HippocampusSql.Interfaces;
using HippocampusSql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HippocampusSql.Services
{
    /// <summary>
    /// SQL Query builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : ISqlBuilder<T>
        where T : class
    {
        public ICollection<ISqlDefinition> Definitions { get; }

        /// <summary>
        /// Create a new <see cref="SqlBuilder{T}"/>
        /// </summary>
        public SqlBuilder()
        {
            Definitions = new List<ISqlDefinition>();
        }

        static SqlBuilder()
        {
            LamdaMethod = typeof(Expression)
            .GetMethods()
            .First(m => m.Name == nameof(Expression.Lambda)
                        && typeof(Expression).IsAssignableFrom(m.ReturnType)
                        && m.GetParameters()
                            .Select(p => p.ParameterType)
                            .SequenceEqual(new[] { typeof(Expression), typeof(ParameterExpression[]) })
            );
        }

        protected static readonly MethodInfo LamdaMethod;


        #region ISqlBuilder implementation

        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="selector">Expression to return a object array of the columns(props) that you want to select</param>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Select(Expression<Func<T, object[]>> selector, TableInformation? info = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a select query that fetches all mapped columns
        /// </summary>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Select(TableInformation? info = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a insert query
        /// </summary>
        /// <param name="entity">Entity to be inserted</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Insert(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a update query
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Update(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a delete query
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Delete(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a where over this query
        /// </summary>
        /// <param name="predicate">A predicate to iterate over the query</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the sql string made out of this builder
        /// </summary>
        /// <returns>SQL string</returns>
        public SqlQuery Materialize()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Use <see cref="Materialize"/>
        /// </summary>
        /// <returns>SQL string</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }
        #endregion

        public ISqlBuilder<T> AppendDefinition(ISqlDefinition definition)
        {
            Definitions.Add(definition);
            return this;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}