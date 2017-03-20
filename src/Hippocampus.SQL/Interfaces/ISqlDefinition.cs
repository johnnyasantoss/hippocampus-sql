using System.Text;

namespace HippocampusSql.Interfaces
{
    public interface ISqlDefinition
    {
        ISqlStatement Statement { get; }

        StringBuilder AppendSqlInto(StringBuilder strBuilder);

        string ToSql();
    }
}
