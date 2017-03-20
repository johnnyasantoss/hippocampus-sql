using HippocampusSql.Interfaces;
using HippocampusSql.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace HippocampusSql
{
    public class ClassMetadata : IClassMetadata
    {
        /// <summary>
        /// Creates a chace of a table info
        /// </summary>
        /// <param name="type">Type of the mapped class</param>
        public ClassMetadata(Type type)
        {
            var props = type.GetProperties();
            var table = type
                .GetTypeInfo()
                .GetCustomAttribute<TableAttribute>();

            if (table == null)
                TableInfo = new TableInformation(type.Name);
            else
                TableInfo = new TableInformation(table.Name, table.Schema);

            foreach (var prop in props)
            {
                var isKey = prop.GetCustomAttribute(typeof(KeyAttribute)) != null;
                var columnInfo = (ColumnAttribute)prop.GetCustomAttribute(typeof(ColumnAttribute));

                var name = columnInfo?.Name ?? prop.Name;

                if (isKey)
                    Keys.Add(name);
                Columns.Add(name);
            }
        }

        /// <summary>
        /// Mapped columns
        /// </summary>
        public List<string> Columns { get; } = new List<string>();

        /// <summary>
        /// Mapped Keys
        /// </summary>
        public List<string> Keys { get; } = new List<string>();

        /// <summary>
        /// Table informations
        /// </summary>
        public TableInformation TableInfo { get; }

        IEnumerable<string> IClassMetadata.Columns
            => Columns;

        IEnumerable<string> IClassMetadata.Keys
            => Keys;
    }
}