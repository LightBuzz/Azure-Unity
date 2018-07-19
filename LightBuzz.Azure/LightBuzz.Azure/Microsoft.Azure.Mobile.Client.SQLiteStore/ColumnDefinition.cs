#if !UNITY_WSA
// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.MobileServices.SQLiteStore
{
    /// <summary>
    /// A class that represents the type of column on local store.
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The JSON type of the column.
        /// </summary>
        public JTokenType JsonType { get; private set; }

        /// <summary>
        /// The data type of the column.
        /// </summary>
        public string StoreType { get; private set; }

        /// <summary>
        /// Creates a new table <see cref="ColumnDefinition"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="jsonType">The JSON type of the column.</param>
        /// <param name="storeType">The data type of the column.</param>
        public ColumnDefinition(string name, JTokenType jsonType, string storeType)
        {
            Name = name;
            JsonType = jsonType;
            StoreType = storeType;
        }

        /// <summary>
        /// Creates a hash value of the table column definition.
        /// </summary>
        /// <returns>The has code value.</returns>
        public override int GetHashCode()
        {
            return Tuple.Create(Name, JsonType, StoreType).GetHashCode();
        }

        /// <summary>
        /// Determines whether the current column definition instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the column definitions are equal. False otherwise.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as ColumnDefinition;
            if (other == null)
            {
                return base.Equals(obj);
            }

            return Name.Equals(other.Name) &&
                   JsonType.Equals(other.JsonType) &&
                   StoreType.Equals(other.StoreType);
        }

        /// <summary>
        /// Creates a string representation of the current table column definition.
        /// </summary>
        /// <returns>A string representation of the column information.</returns>
        public override string ToString()
        {
            return String.Format("{0}, {1}, {1}", Name, JsonType, StoreType);
        }
    }
}
#endif