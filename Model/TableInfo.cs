namespace HippocampusSql.Model
{
    internal class TableInfo
    {
        internal string Name { get; set; }
        internal string Abrv { get; set; }

        public TableInfo(string name, string abrv)
        {
            Name = name;
            Abrv = abrv;
        }
    }
}