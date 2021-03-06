﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Hippocampus.SQL.Interfaces;
using Hippocampus.SQL.Model;

namespace Hippocampus.SQL
{
    /// <summary>
    /// Stores mapped class info
    /// </summary>
    public class ClassMetadataCache : IClassMetadataCache
    {
        /// <summary>
        /// Creates a cache of a table info
        /// </summary>
        /// <param name="type">Type of the mapped class</param>
        public ClassMetadataCache(Type type)
        {
            var props = type.GetRuntimeProperties();
            var table = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();

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
        /// Table information
        /// </summary>
        public TableInformation TableInfo { get; }

        IEnumerable<string> IClassMetadataCache.Columns
            => Columns;

        IEnumerable<string> IClassMetadataCache.Keys
            => Keys;
    }
}