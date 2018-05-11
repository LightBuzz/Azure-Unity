// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;

namespace Microsoft.WindowsAzure.MobileServices.SQLiteStore
{
    public class SQLiteException : Exception
    {
        public SQLiteException(string message)
            : base(message)
        {
        }
    }
}
