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
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == 0).Materialize();

            const string expected = @"SELECT * FROM dbo.TEST
WHERE (Id = @p1)";

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(expected, where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 2 where expressions
        /// </summary>
        [Fact]
        public void CanGenerateWhereWithConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == 0 && t.Name != null).Materialize();

            const string expected = @"SELECT * FROM dbo.TEST
WHERE (Id = @p1
 AND Name <> null)";

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(expected, where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 2 fluent where expressions
        /// </summary>
        [Fact]
        public void CanGenerateWhereWithFluentConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == 0)
                .Where(t => t.Name != null)
                .Materialize();

            const string expected = @"SELECT * FROM dbo.TEST
WHERE (Id = @p1)
 AND (Name <> null)";

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(expected, where, true, true, true);
        }

        /// <summary>
        /// Checks if 
        /// <see cref="SqlBuilder{T}.Select(System.Linq.Expressions.Expression{System.Func{T, object[]}}, Model.TableInformation?)" />
        /// can generate a one column select
        /// </summary>
        [Fact]
        public void CanGenerateSelectOfOneColumn()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var sql = sqlBuilder
                .Select(t => new object[]
                {
                    t.Id
                })
                .Materialize();

            Assert.NotNull(sql);
            Assert.NotEmpty(sql);
            Assert.Equal("SELECT Id FROM dbo.TEST", sql, true, true, true);
        }

        /// <summary>
        /// Checks if 
        /// <see cref="SqlBuilder{T}.Select(System.Linq.Expressions.Expression{System.Func{T, object[]}}, Model.TableInformation?)" />
        /// can generate a everything select
        /// </summary>
        [Fact]
        public void CanGenerateSelectWithWildCard()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var sql = sqlBuilder
                .Select()
                .Materialize();

            Assert.NotNull(sql);
            Assert.NotEmpty(sql);
            Assert.Equal("SELECT * FROM dbo.TEST", sql, true, true, true);
        }
    }
}