using System.Text;

namespace HippocampusSql.Interfaces
{
    public interface ISqlDefinition
    {
        StringBuilder AppendSqlInto(StringBuilder strBuilder);

        string ToSql();
    }
}
