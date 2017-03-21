using HippocampusSql.Definitions;
using HippocampusSql.Interfaces;
using HippocampusSql.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HippocampusSql.Services
{
    public static class ExpressionResolver
    {
        static ExpressionResolver()
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

        private static readonly MethodInfo LamdaMethod;


        public static IEnumerable<ISqlDefinition> ResolveSelect(ISelectDefinition selectDef, Expression<Func<object[]>> columns)
        {
            columns.CheckNull(nameof(columns));

            if (columns.Body is NewArrayExpression arrExp)
                return ResolveNewArrayExpression(arrExp, selectDef.Statement);
            else
                throw new InvalidOperationException("Columns selector expression should be an NewArrayExpression.");
        }

        private static IEnumerable<ISqlDefinition> ResolveExpression(Expression exp, ISqlStatement statement)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp), "An expression should be provided.");

            Debug.WriteLine($"Resolving expression: {exp.GetType().FullName}:{exp}", "Info");

            if (exp is LambdaExpression lamdaExp)
            {
                return ResolveExpression(lamdaExp.Body, statement);
            }
            else if (exp is BinaryExpression bExp)
            {
                return ResolveBinaryExpression(bExp, statement);
            }
            else if (exp is MemberExpression memberExp)
            {
                return ResolveMemberExpression(memberExp, statement);
            }
            else if (exp is UnaryExpression unaryExp)
            {
                return ResolveExpression(unaryExp.Operand, statement);
            }
            else if (exp is ConstantExpression constExp)
            {
                string paramKey;

                if (constExp.Value == null)
                    paramKey = "null";
                else if (constExp.Value is char && ((char)constExp.Value) == '*')
                    paramKey = "*";
                else
                    paramKey = statement.GenerateNewParameter(constExp.Value);

                return new ISqlDefinition[1] { new SelectItemDefinition(statement, paramKey) };
            }
            else if (exp is ParameterExpression)
            {
                //QueryInfo.AppendInto(appendType, s => s.Append(((ParameterExpression)exp).Name));
                throw new NotImplementedException();
            }
            else if (exp is NewArrayExpression newArrExp)
            {
                return ResolveNewArrayExpression(newArrExp, statement);
            }
            else
                throw new NotImplementedException("Expression not implemented.");
        }

        private static IEnumerable<ISqlDefinition> ResolveNewArrayExpression(NewArrayExpression expression
            , ISqlStatement statement
            )
        {
            var list = new List<IEnumerable<ISqlDefinition>>();

            foreach (var exp in expression.Expressions)
                list.Add(ResolveExpression(exp, statement));

            return list.SelectMany(l => l);
        }

        private static IEnumerable<ISqlDefinition> ResolveBinaryExpression(BinaryExpression exp, ISqlStatement statement)
        {
            var left = ResolveExpression(exp.Left, statement);

            ISqlDefinition middle;
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    middle = new AndDefinition(statement);
                    break;
                case ExpressionType.OrElse:
                    middle = new OrElseDefinition(statement);
                    break;
                case ExpressionType.Equal:
                    middle = new EqualDefinition(statement);
                    break;
                case ExpressionType.NotEqual:
                    middle = new NotEqualDefinition(statement);
                    break;
                default:
                    throw new NotSupportedException($"The expression type \"{exp.NodeType}\" not supported yet.");
            }

            var right = ResolveExpression(exp.Right, statement);

            return left
                .Concat(new[] { middle })
                .Concat(right);
        }

        private static IEnumerable<ISqlDefinition> ResolveMemberExpression(MemberExpression exp, ISqlStatement statement)
        {
            var memberType = exp.Member.DeclaringType;
            var cache = statement.ClassCache[memberType];

            var expTypeName = new Lazy<string>(() => exp.GetType().Name);

            var memberName = exp.Member.Name;

            if (cache.Columns.Contains(memberName))
            {
                return new[] { new ColumnDefinition(statement, memberName, cache) };
            }
            else if (typeof(IEnumerable<string>).IsAssignableFrom(exp.Type))
            {
                var columns = Expression.Lambda<Func<IEnumerable<string>>>(exp)
                    .Compile()()
                    .ToArray();

                var len = columns.Length;
                var definitions = new ISqlDefinition[len];

                bool isFirst = true;
                for (var i = 0; i < len; i++)
                {
                    if (isFirst && i >= 1)
                        isFirst = false;
                    definitions[i] = new ColumnDefinition(statement, memberName, cache, isFirst: true);
                }

                return definitions;
            }
            else if (expTypeName.Value == "FieldExpression"
                     || expTypeName.Value == "PropertyExpression")
            {
                var value = GetValueFromExpression(exp);

                var paramKey = value == null ? "null" : statement.GenerateNewParameter(value);

                return new[] { new ColumnDefinition(statement, paramKey, cache) };
            }
            else
                throw new InvalidOperationException($"The expression \"{exp}\" to prop that's not a column.");
        }

        private static object GetValueFromExpression(MemberExpression exp)
        {
            // TODO: Implement a cache system for this make generics
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
    }
}
