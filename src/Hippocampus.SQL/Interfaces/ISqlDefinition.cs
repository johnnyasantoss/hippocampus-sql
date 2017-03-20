using System.Text;

namespace HippocampusSql.Interfaces
{
    public interface ISqlDefinition
    {
        IClassMetadataCache ClassCache { get; }

        ISqlStatement Statement { get; }

        StringBuilder AppendSqlInto(StringBuilder strBuilder);

        string ToSql();
    }
}
