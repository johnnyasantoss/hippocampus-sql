using System.Text;

namespace HippocampusSql.Interfaces
{
    public interface ISqlDefinition
    {
        ISqlStatmentInfo Info { get; }

        StringBuilder AppendSqlInto(StringBuilder strBuilder);

        string ToSql();
    }
}
