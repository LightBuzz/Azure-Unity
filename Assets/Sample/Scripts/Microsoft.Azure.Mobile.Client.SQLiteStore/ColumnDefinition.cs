// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.MobileServices.SQLiteStore
{
    /// <summary>
    /// A class that represents the type of column on local store
    /// </summary>
    public class ColumnDefinition
    {
        public string Name { get; private set; }

        public JTokenType JsonType { get; private set; }

        public string StoreType { get; private set; }


        public ColumnDefinition(string name, JTokenType jsonType, string storeType)
        {
            Name = name;
            JsonType = jsonType;
            StoreType = storeType;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Name, JsonType, StoreType).GetHashCode();
        }

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

        public override string ToString()
        {
            return String.Format("{0}, {1}, {1}", Name, JsonType, StoreType);
        }
    }
}
