using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using HippocampusSql.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HippocampusSql.Services
{
    /// <summary>
    /// SQL builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : ISqlBuilder<T>
        where T : class
    {
        internal IClassMetadataCache ClassCache { get; private set; }

        private ISqlQuery _query { get; }

        public bool Beautify { get; set; } = true;

        /// <summary>
        /// Create a new <see cref="SqlBuilder{T}"/>
        /// </summary>
        public SqlBuilder(QueryType queryType)
        {
            _query = new SqlQuery(queryType);
            var tableInfo = typeof(T);
            _query.TableInfo = new TableInfo(tableInfo.Name, tableInfo.Name);

            InitializeCache();
        }

        private void InitializeCache()
        {
            var props = typeof(T).GetProperties();
            var cache = new ClassMetadataCache();

            foreach (var prop in props)
            {
                var isKey = prop.GetCustomAttribute(typeof(KeyAttribute)) != null;
                var columnInfo = (ColumnAttribute)prop.GetCustomAttribute(typeof(ColumnAttribute));

                var name = columnInfo?.Name ?? prop.Name;

                if (isKey)
                    cache.Keys.Add(name);
                cache.Columns.Add(name);
            }

            ClassCache = cache;
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

                _query.TableInfo.Abrv = lamdaExp.Parameters[0].Name;

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
                var paramKey = constExp.Value == null
                    ? "null"
                    : _query.GenerateNewParameter(constExp.Value);

                _query.AppendInto(appendType, s => s.Append(paramKey));
            }
            else if (exp is ParameterExpression)
            {
                _query.AppendInto(appendType, s => s.Append(((ParameterExpression)exp).Name));
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private void ResolveBinaryExpression(BinaryExpression exp, AppendType appendType)
        {
            ResolveExpression(exp.Left, appendType);

            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    _query.AppendInto(appendType,
                        s => s.AppendLine()
                              .Append(" AND ")
                        );
                    break;
                case ExpressionType.OrElse:
                    _query.AppendInto(appendType,
                        s => s.AppendLine()
                              .Append(" OR ")
                        );
                    break;
                case ExpressionType.Equal:
                    _query.AppendInto(appendType,
                        s => s.Append(" = ")
                        );
                    break;
                case ExpressionType.NotEqual:
                    _query.AppendInto(appendType,
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
            if (ClassCache.Columns.Contains(exp.Member.Name))
            {
                ResolveExpression(exp.Expression, appendType);
                _query.AppendInto(appendType,
                    s => s.Append('.').Append(exp.Member.Name)
                    );
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }
        #endregion

        #region ISqlBuilde implementation

        public ISqlBuilder<T> Select(Expression<Func<T, object[]>> selector)
        {
            throw new NotImplementedException();
        }

        public ISqlBuilder<T> Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public ISqlBuilder<T> Update(T entity)
        {
            throw new NotImplementedException();
        }

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
            using (_query.BeginWhere())
                ResolveExpression(predicate, AppendType.Where);

            return this;
        }

        /// <summary>
        /// Generates the sql string made out of this builder
        /// </summary>
        /// <returns>SQL string</returns>
        public string Materialize() => _query.ToSqlString();

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