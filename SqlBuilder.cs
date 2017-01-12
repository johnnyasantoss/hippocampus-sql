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

        /// <summary>
        /// Create a new <see cref="SqlBuilder{T}"/>
        /// </summary>
        public SqlBuilder()
        {
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

        public string GetAllWhere(Expression<Func<T, bool>> predicate)
        {
            var query = new SqlQuery(QueryType.Select);

            ResolveLambdaExpression(predicate, query);

            return query.ToSqlString();
        }

        private void ResolveLambdaExpression(Expression exp, ISqlQuery query = null)
        {
            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression)
            {
                var lamdaExp = ((LambdaExpression)exp);

                if (query.TableInfo == null)
                {
                    var tableInfo = lamdaExp.Parameters[0];
                    query.TableInfo = new TableInfo(tableInfo.Type.Name, tableInfo.Name);
                }

                ResolveLambdaExpression(lamdaExp.Body, query);
            }
            else if (exp is BinaryExpression)
            {
                ResolveBinaryExpression((BinaryExpression)exp, query);
            }
            else if (exp is MemberExpression)
            {
                ResolveMemberExpression((MemberExpression)exp, query);
            }
            else if (exp is UnaryExpression)
            {
                ResolveLambdaExpression(((UnaryExpression)exp).Operand, query);
            }
            else if (exp is ConstantExpression)
            {
                var constExp = (ConstantExpression)exp;
                var paramKey = query.GenerateNewParameter(constExp.Value);
                query.Where.Append(paramKey);
            }
            else if (exp is ParameterExpression)
            {
                query.Where.Append(((ParameterExpression)exp).Name);
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private void ResolveBinaryExpression(BinaryExpression exp, ISqlQuery query)
        {
            ResolveLambdaExpression(exp.Left, query);

            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    query.Where
                        .AppendLine()
                        .Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    query.Where
                        .AppendLine()
                        .Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    query.Where.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    query.Where.Append(" <> ");
                    break;
                default:
                    throw new NotSupportedException($"The expression type \"{exp.NodeType}\" not supported yet.");
            }

            ResolveLambdaExpression(exp.Right, query);
        }

        private void ResolveMemberExpression(MemberExpression exp, ISqlQuery query)
        {
            if (ClassCache.Columns.Contains(exp.Member.Name))
            {
                ResolveLambdaExpression(exp.Expression, query);
                query.Where
                    .Append('.')
                    .Append(exp.Member.Name);
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }
    }
}