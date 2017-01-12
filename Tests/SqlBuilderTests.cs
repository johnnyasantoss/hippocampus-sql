using Xunit;

namespace HippocampusSql.Tests
{
    public class SqlBuilderTests
    {
        [Fact]
        public void CanGenerateWhere()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder.GetAllWhere(t => t.Id == 0);

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(@"SELECT * FROM TestClass AS t 
WHERE t.Id = @p1
", where, true, true, true);
        }

        [Fact]
        public void CanGenerateWhereWithConcatenation()
        {
            var sqlBuilder = new SqlBuilder<TestClass>();

            var where = sqlBuilder.GetAllWhere(t => t.Id == 0 && t.Name != null);

            Assert.NotNull(where);
            Assert.NotEmpty(where);
            Assert.Equal(@"SELECT * FROM TestClass AS t 
WHERE t.Id = @p1
 AND t.Name <> @p2
", where, true, true, true);
        }
    }
}