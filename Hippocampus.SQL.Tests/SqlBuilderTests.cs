using HippocampusSql.Services;
using Xunit;

namespace HippocampusSql.Tests
{
    /// <summary>
    /// Tests for <see cref = "SqlBuilder{T}" />
    /// </summary >
    public class SqlBuilderTests
    {
        /// <summary>
        /// Checks if <see cref = "SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})" />
        /// can generate a simple query with 1 where expression only
        /// </summary>
        [Fact]
        public void CanGenerateWhere()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == 0)
                .Materialize();

            const string expected = @"SELECT Id
, Name
 FROM dbo.TEST
WHERE (Id = @p1)";

            Assert.NotNull(where);
            Assert.Equal(expected, where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref="SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})"/>
        /// can generate a simple query with 1 where expression using a value from parameter
        /// </summary>
        [Theory]
        [InlineData(1)]
        public void CanGenerateWhereWhenAValueIsProvidedFromAnotherMethod(int id)
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == id)
                .Materialize();

            const string expected = @"SELECT Id
, Name
 FROM dbo.TEST
WHERE (Id = @p1)";

            Assert.NotNull(where);
            Assert.Equal(expected, where, true, true, true);
        }

        /// <summary>
        /// Checks if <see cref = "SqlBuilder{T}.Where(System.Linq.Expressions.Expression{System.Func{T, bool}})" />
        /// can generate a simple query with 2 where expressions
        /// </summary>
        [Fact]
        public void CanGenerateWhereWithConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder
                .Select()
                .Where(t => t.Id == 0 && t.Name != null)
                .Materialize();

            const string expected = @"SELECT Id
, Name
 FROM dbo.TEST
WHERE (Id = @p1
 AND Name <> null)";

            Assert.NotNull(where);
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

            const string expected = @"SELECT Id
, Name
 FROM dbo.TEST
WHERE (Id = @p1)
 AND (Name <> null)";

            Assert.NotNull(where);
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

            const string expected = @"SELECT Id
 FROM dbo.TEST";

            Assert.NotNull(sql);
            Assert.Equal(expected, sql, true, true, true);
        }

        /// <summary>
        /// Checks if 
        /// <see cref="SqlBuilder{T}.Select(System.Linq.Expressions.Expression{System.Func{T, object[]}}, Model.TableInformation?)" />
        /// can generate a everything select
        /// </summary>
        [Fact]
        public void CanGenerateSelect()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var sql = sqlBuilder
                .Select()
                .Materialize();

            const string expected = @"SELECT Id
, Name
 FROM dbo.TEST";

            Assert.NotNull(sql);
            Assert.Equal(expected, sql, true, true, true);
        }
    }
}