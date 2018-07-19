#if !UNITY_WSA
// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;

namespace Microsoft.WindowsAzure.MobileServices.SQLiteStore
{
    /// <summary>
    /// Represents a SQLite exception.
    /// </summary>
    public class SQLiteException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="SQLiteException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SQLiteException(string message)
            : base(message)
        {
        }
    }
}
#endif