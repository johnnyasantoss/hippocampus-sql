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
                            .SequenceEqual(new[] {typeof(Expression), typeof(ParameterExpression[])})
            );
    }

    /// <summary>
    /// SQL Query builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : SqlBuilder, ISqlBuilder<T>
        where T : class
    {
        private ISqlQueryInfo QueryInfo { get; }

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


        #region Expression resolvers

        private void ResolveExpression(Expression exp, AppendType appendType)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp), "An expression should be provided.");

            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression)
            {
                var lamdaExp = ((LambdaExpression) exp);

                ResolveExpression(lamdaExp.Body, appendType);
            }
            else if (exp is BinaryExpression)
            {
                ResolveBinaryExpression((BinaryExpression) exp, appendType);
            }
            else if (exp is MemberExpression)
            {
                ResolveMemberExpression((MemberExpression) exp, appendType);
            }
            else if (exp is UnaryExpression)
            {
                ResolveExpression(((UnaryExpression) exp).Operand, appendType);
            }
            else if (exp is ConstantExpression)
            {
                var constExp = (ConstantExpression) exp;
                string paramKey;

                if (constExp.Value == null)
                    paramKey = "null";
                else if (constExp.Value is char && ((char) constExp.Value) == '*')
                    paramKey = "*";
                else
                    paramKey = QueryInfo.GenerateNewParameter(constExp.Value);

                QueryInfo.AppendInto(appendType, s => s.Append(paramKey));
            }
            else if (exp is ParameterExpression)
            {
                QueryInfo.AppendInto(appendType, s => s.Append(((ParameterExpression) exp).Name));
            }
            else if (exp is NewArrayExpression)
            {
                ResolveNewArrayExpression((NewArrayExpression) exp, appendType);
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
                    QueryInfo.AppendInto(appendType,
                        s => s.AppendLine()
                            .Append(" AND ")
                    );
                    break;
                case ExpressionType.OrElse:
                    QueryInfo.AppendInto(appendType,
                        s => s.AppendLine()
                            .Append(" OR ")
                    );
                    break;
                case ExpressionType.Equal:
                    QueryInfo.AppendInto(appendType,
                        s => s.Append(" = ")
                    );
                    break;
                case ExpressionType.NotEqual:
                    QueryInfo.AppendInto(appendType,
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
            var cache = QueryInfo.ClassCache;

            var expTypeName = new Lazy<string>(() => exp.GetType().Name);
            if (cache.Columns.Contains(exp.Member.Name))
            {
                AppendColumn(exp.Member.Name, appendType, cache.TableInfo);
            }
            else if (typeof(IEnumerable<string>).IsAssignableFrom(exp.Type))
            {
                var columns = Expression.Lambda<Func<IEnumerable<string>>>(exp)
                    .Compile()()
                    .ToArray();

                var len = columns.Length;

                for (var i = 0; i < len; i++)
                {
                    if (i >= 1)
                        QueryInfo.AppendInto(appendType, s => s.AppendLine().Append(", "));
                    AppendColumn(columns[i], appendType, cache.TableInfo);
                }
            }
            else if (expTypeName.Value == "FieldExpression"
                     || expTypeName.Value == "PropertyExpression")
            {
                var value = GetValueFromExpression(exp);

                var paramKey = value == null ? "null" : QueryInfo.GenerateNewParameter(value);

                QueryInfo.AppendInto(appendType, s => s.Append(paramKey));
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }

        private static object GetValueFromExpression(MemberExpression exp)
        {
            var mLamda = LamdaMethod
                .MakeGenericMethod(
                    typeof(Func<>)
                        .MakeGenericType(exp.Type)
                );

            var mCompile = mLamda.ReturnType
                .GetMethods()
                .First(m => m.Name == nameof(Expression<int>.Compile)
                            && !m.GetParameters().Any());

            var expression = mLamda.Invoke(null, new object[] {exp, null});

            var func = (Delegate) mCompile.Invoke(expression, new object[0]);

            var value = func.DynamicInvoke();

            return value;
        }

        private void AppendColumn(string columnName, AppendType appendType, TableInformation info)
        {
            if (!string.IsNullOrWhiteSpace(info.Abbreviation))
            {
                QueryInfo.AppendInto(appendType, s => s.Append(info.Abbreviation).Append('.'));
            }

            QueryInfo.AppendInto(appendType, s => s.Append(columnName));
        }

        #endregion

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