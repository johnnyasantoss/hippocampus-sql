namespace HippocampusSql.Interfaces
{
    public interface ISelectItem : ISqlDefinition
    {
        string Expression { get; }

        string Alias { get; }
    }
}
