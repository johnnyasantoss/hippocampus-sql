﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Hippocampus.SQL.Enums;
using Hippocampus.SQL.Interfaces;
using Hippocampus.SQL.Model;

namespace Hippocampus.SQL.Services
{
    /// <summary>
    /// SQL builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : ISqlBuilder<T>
        where T : class
    {
        private ISqlQuery Query { get; }

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
            Query = new SqlQuery(
                new ClassMetadataCache(typeof(T))
                , Beautify
                );
        }


        #region Expression resolvers
        private void ResolveExpression(Expression exp, AppendType appendType)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp), "An expression should be provided.");

            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression)
            {
                var lamdaExp = ((LambdaExpression)exp);

                ResolveExpression(lamdaExp.Body, appendType);
            }
            else if (exp is BinaryExpression)
            {
                ResolveBinaryExpression((BinaryExpression)exp, appendType);
            }
            else if (exp is MemberExpression)
            {
                ResolveMemberExpression((MemberExpression)exp, appendType);
            }
            else if (exp is UnaryExpression)
            {
                ResolveExpression(((UnaryExpression)exp).Operand, appendType);
            }
            else if (exp is ConstantExpression)
            {
                var constExp = (ConstantExpression)exp;
                string paramKey;

                if (constExp.Value == null)
                    paramKey = "null";
                else if (constExp.Value is char && ((char)constExp.Value) == '*')
                    paramKey = "*";
                else
                    paramKey = Query.GenerateNewParameter(constExp.Value);

                Query.AppendInto(appendType, s => s.Append(paramKey));
            }
            else if (exp is ParameterExpression)
            {
                Query.AppendInto(appendType, s => s.Append(((ParameterExpression)exp).Name));
            }
            else if (exp is NewArrayExpression)
            {
                ResolveNewArrayExpression(exp as NewArrayExpression, appendType);
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private void ResolveNewArrayExpression(NewArrayExpression exp, AppendType appendType)
        {
            switch (appendType)
            {
                case AppendType.Select:
                    foreach (var e in exp.Expressions)
                        ResolveExpression(e, appendType);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ResolveBinaryExpression(BinaryExpression exp, AppendType appendType)
        {
            ResolveExpression(exp.Left, appendType);

            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    Query.AppendInto(appendType,
                        s => s.AppendLine()
                              .Append(" AND ")
                        );
                    break;
                case ExpressionType.OrElse:
                    Query.AppendInto(appendType,
                        s => s.AppendLine()
                              .Append(" OR ")
                        );
                    break;
                case ExpressionType.Equal:
                    Query.AppendInto(appendType,
                        s => s.Append(" = ")
                        );
                    break;
                case ExpressionType.NotEqual:
                    Query.AppendInto(appendType,
                        s => s.Append(" <> ")
                        );
                    break;
                default:
                    throw new NotSupportedException($"The expression type \"{exp.NodeType}\" not supported yet.");
            }

            ResolveExpression(exp.Right, appendType);
        }

        private void ResolveMemberExpression(MemberExpression exp, AppendType appendType)
        {
            var cache = Query.ClassCache;
            if (cache.Columns.Contains(exp.Member.Name))
            {
                if (!string.IsNullOrWhiteSpace(cache.TableInfo.Abbreviation))
                {
                    ResolveExpression(exp.Expression, appendType);
                    Query.AppendInto(appendType, s => s.Append('.'));
                }

                Query.AppendInto(appendType, s => s.Append(exp.Member.Name));
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }
        #endregion

        #region ISqlBuilde implementation
        /// <summary>
        /// Generates a select query
        /// </summary>
        /// <param name="selector">Expression to return a object array of the columns(props) that you want to select</param>
        /// <param name="info">Information of the table that will be selected</param>
        /// <returns>This builder</returns>
        public ISqlBuilder<T> Select(Expression<Func<T, object[]>> selector, TableInformation? info = null)
        {
            using (Query.BeginSelect())
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
            using (Query.BeginSelect())
            {
                Expression<Func<char>> exp = () => '*';
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
            using (Query.BeginWhere())
                ResolveExpression(predicate, AppendType.Where);

            return this;
        }

        /// <summary>
        /// Generates the sql string made out of this builder
        /// </summary>
        /// <returns>SQL string</returns>
        public string Materialize() => Query.ToSqlString();

        #endregion

        #region Overrides
        /// <summary>
        /// Use <see cref="Materialize"/>
        /// </summary>
        /// <returns>SQL string</returns>
        [Obsolete]
        [SuppressMessage("Warning", "CS0809", Justification = "A fallback for those who don't know Materialize() method.")]
        public override string ToString()
            => Materialize();
        #endregion
    }
}