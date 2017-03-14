using HippocampusSql.Interfaces;
using HippocampusSql.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace HippocampusSql.Services
{
    public static class ExpressionResolver
    {
        public static IEnumerable<ISelectDefinition> ResolveSelect(Expression<Func<object[]>> columns)
        {
            columns.CheckArgumentNull(nameof(columns));

            if (columns.Body is NewArrayExpression)
                ResolveNewArrayExpression(columns.Body as NewArrayExpression);
            else
                throw new InvalidOperationException("Columns selector expression should be an NewArrayExpression.");
        }

        private void ResolveExpression(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp), "An expression should be provided.");

            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression)
            {
                var lamdaExp = ((LambdaExpression)exp);

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
                string paramKey;

                if (constExp.Value == null)
                    paramKey = "null";
                else if (constExp.Value is char && ((char)constExp.Value) == '*')
                    paramKey = "*";
                else
                    paramKey = QueryInfo.GenerateNewParameter(constExp.Value);

                QueryInfo.AppendInto(appendType, s => s.Append(paramKey));
            }
            else if (exp is ParameterExpression)
            {
                QueryInfo.AppendInto(appendType, s => s.Append(((ParameterExpression)exp).Name));
            }
            else if (exp is NewArrayExpression)
            {
                ResolveNewArrayExpression((NewArrayExpression)exp, appendType);
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private static void ResolveNewArrayExpression(NewArrayExpression expression)
        {
            foreach (var exp in expression.Expressions)
                ResolveExpression(exp);
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

            var expression = mLamda.Invoke(null, new object[] { exp, null });

            var func = (Delegate)mCompile.Invoke(expression, new object[0]);

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
    }
}
