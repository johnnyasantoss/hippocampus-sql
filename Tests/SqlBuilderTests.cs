using HippocampusSql.Enums;
using HippocampusSql.Services;
using Xunit;

namespace HippocampusSql.Tests
{
    /// <summary>
    /// Tests for <see cref="SqlBuilder{T}"/>
    /// </summary>
    public class SqlBuilderTests
    {
        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 1 where expression only
        /// </summary>
        [Fact]
        public void CanGenerateWhere()
        {
            var sqlBuilder = new SqlBuilder<TestClass>(QueryType.Select);

            var where = sqlBuilder.Where(t => t.Id == 0).Materialize();

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(@"SELECT * FROM TestClass AS t 
WHERE (t.Id = @p1)
", where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 2 where expressions
        /// </summary>
        [Fact]
        public void CanGenerateWhereWithConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>(QueryType.Select);

            var where = sqlBuilder.Where(t => t.Id == 0 && t.Name != null).Materialize();

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(@"SELECT * FROM TestClass AS t 
WHERE (t.Id = @p1
 AND t.Name <> null)
", where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 2 fluent where expressions
        /// </summary>
        [Fact]
        public void CanGenerateWhereWithFluentConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>(QueryType.Select);

            var where = sqlBuilder
                .Where(t => t.Id == 0)
                .Where(t => t.Name != null)
                .Materialize();

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(@"SELECT * FROM TestClass AS t 
WHERE (t.Id = @p1)
 AND (t.Name <> null)
", where, true, true, true);
        }
    }
}