namespace HippocampusSql.Interfaces
{
    public interface IColumnDefinition : ISqlDefinition
    {
        bool IsFirst { get; }

        bool IsLast { get; }
    }
}
