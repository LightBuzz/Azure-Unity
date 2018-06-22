//
// Copyright (c) LightBuzz Software.
// All rights reserved.
//
// http://lightbuzz.com
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
// COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
// OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
// AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//

using System.Threading;
using System.Threading.Tasks;
using LightBuzz.Azure;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

/// <summary>
/// A subclass that extends LightBuzzMobileServiceClient.
/// </summary>
class SampleMobileClient : LightBuzzMobileServiceClient
{
    /// <summary>
    /// Creates a new instance of the SampleMobileClient. 
    /// </summary>
    /// <param name="mobileAppUri">Azure App Service URL</param>
    /// <param name="supportLocal">Supports local database</param>
    public SampleMobileClient(string mobileAppUri, bool supportLocal) : base(mobileAppUri, supportLocal)
    {
    }

    /// <summary>
    /// The implementation of abstract method DefineTables.
    /// </summary>
    protected override void DefineTables()
    {
        LocalStore.DefineTable<TodoItem>();
    }

    /// <summary>
    /// The implementation of abstract method Pull.
    /// </summary>
    /// <returns></returns>
    public override async Task Pull(CancellationToken ct)
    {
        AppServiceTableDAO<TodoItem> todoTableDao = new AppServiceTableDAO<TodoItem>(this);
        await todoTableDao.Pull(ct, "TodoItems", x => x.Id != null);
    }
}
