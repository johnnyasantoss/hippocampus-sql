using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HippocampusSql.Tests
{
    [Table("TEST", Schema = "dbo")]
    internal class TestClass
    {
        [Key, Column]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }
    }
}