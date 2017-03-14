using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using HippocampusSql.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HippocampusSql.Services
{
    /// <summary>
    /// Use <see cref="SqlBuilder{T}"/>
    /// </summary>
    public class SqlBuilder
    {
        internal SqlBuilder()
        {
        }

        protected static readonly MethodInfo LamdaMethod = typeof(Expression)
            .GetMethods()
            .First(m => m.Name == nameof(Expression.Lambda)
                        && typeof(Expression).IsAssignableFrom(m.ReturnType)
                        && m.GetParameters()
                            .Select(p => p.ParameterType)
                            .SequenceEqual(new[] { typeof(Expression), typeof(ParameterExpression[]) })
            );
    }

    /// <summary>
    /// SQL Query builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : SqlBuilder, ISqlBuilder<T>
        where T : class
    {
        private ISqlStatmentInfo QueryInfo { get; }

        /// <summary>
        /// Defines if query should have line-breaks
        /// </summary>
        public bool Beautify { get; set; }

        /// <summary>
        /// Create a new <see cref="SqlBuilder{T}"/>
        /// </summary>
        public SqlBuilder(bool beautify = true)
        {
            Beautify = beautify;
            QueryInfo = new SqlQueryInfo(
                new ClassMetadataCache(typeof(T))
                , Beautify
            );
        }


        #region ISqlBuilder implementation

        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="selector">Expression to return a object array of the columns(props) that you want to select</param>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Select(Expression<Func<T, object[]>> selector, TableInformation? info = null)
        {
            using (QueryInfo.BeginSelect())
                ResolveExpression(selector, AppendType.Select);

            return this;
        }

        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Select(TableInformation? info = null)
        {
            using (QueryInfo.BeginSelect())
            {
                Expression<Func<IEnumerable<string>>> exp = () => QueryInfo.ClassCache.Columns;
                ResolveExpression(exp, AppendType.Select);
            }

            return this;
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
            using (QueryInfo.BeginWhere())
                ResolveExpression(predicate, AppendType.Where);

            return this;
        }

        /// <summary>
        /// Generates the sql string made out of this builder
        /// </summary>
        /// <returns>SQL string</returns>
        public SqlQuery Materialize() => new SqlQuery
        {
            Content = QueryInfo.ToSqlString(),
            NamedParameters = QueryInfo.Parameters
        };

        #endregion

        #region Overrides

        /// <summary>
        /// Use <see cref="Materialize"/>
        /// </summary>
        /// <returns>SQL string</returns>
        public override string ToString()
            => QueryInfo.ToSqlString();

        #endregion
    }
}