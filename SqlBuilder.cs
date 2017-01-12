using HippocampusSql.Enums;
using HippocampusSql.Interfaces;
using HippocampusSql.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HippocampusSql
{
    /// <summary>
    /// SQL builder of a class
    /// </summary>
    /// <typeparam name="T">Type of mapped class to create query from it.</typeparam>
    public class SqlBuilder<T> : ISqlBuilder<T>
        where T : class
    {
        /// <summary>
        /// Stores info from class
        /// </summary>
        public IClassMetadataCache ClassCache { get; private set; }

        private ISqlQuery _query { get; }

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

        public ISqlBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            ResolveExpression(predicate);

            return this;
        }

        private void ResolveExpression(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp), "An expression should be provided.");

            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression)
            {
                var lamdaExp = ((LambdaExpression)exp);

                _query.TableInfo.Abrv = lamdaExp.Parameters[0].Name;

                ResolveExpression(lamdaExp.Body);
            }
            else if (exp is BinaryExpression)
            {
                ResolveBinaryExpression((BinaryExpression)exp);
            }
            else if (exp is MemberExpression)
            {
                ResolveMemberExpression((MemberExpression)exp);
            }
            else if (exp is UnaryExpression)
            {
                ResolveExpression(((UnaryExpression)exp).Operand);
            }
            else if (exp is ConstantExpression)
            {
                var constExp = (ConstantExpression)exp;
                var paramKey = _query.GenerateNewParameter(constExp.Value);
                _query.Where.Append(paramKey);
            }
            else if (exp is ParameterExpression)
            {
                _query.Where.Append(((ParameterExpression)exp).Name);
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private void ResolveBinaryExpression(BinaryExpression exp)
        {
            ResolveExpression(exp.Left);

            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    _query.Where
                        .AppendLine()
                        .Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    _query.Where
                        .AppendLine()
                        .Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    _query.Where.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _query.Where.Append(" <> ");
                    break;
                default:
                    throw new NotSupportedException($"The expression type \"{exp.NodeType}\" not supported yet.");
            }

            ResolveExpression(exp.Right);
        }

        private void ResolveMemberExpression(MemberExpression exp)
        {
            if (ClassCache.Columns.Contains(exp.Member.Name))
            {
                ResolveExpression(exp.Expression);
                _query.Where
                    .Append('.')
                    .Append(exp.Member.Name);
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }

        public string Materialize() => _query.ToSqlString();

        [Obsolete]
        public override string ToString()
            => Materialize();
    }
}