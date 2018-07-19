#if !UNITY_WSA
// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzure.MobileServices.SQLiteStore
{
    /// <summary>
    /// Represents the structure of a table on the local database.
    /// </summary>
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    public class TableDefinition : Dictionary<string, ColumnDefinition>
    {
        /// <summary>
        /// The App Service system properties.
        /// </summary>
        public MobileServiceSystemProperties SystemProperties { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        public TableDefinition()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TableDefinition"/> class with the specified parameters.
        /// </summary>
        /// <param name="definition">The table definition.</param>
        /// <param name="systemProperties">The App Service system properties.</param>
        public TableDefinition(IDictionary<string, ColumnDefinition> definition, MobileServiceSystemProperties systemProperties)
            : base(definition, StringComparer.OrdinalIgnoreCase)
        {
            this.SystemProperties = systemProperties;
        }
    }
}
#endif