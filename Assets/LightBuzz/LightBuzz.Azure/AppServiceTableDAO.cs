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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace LightBuzz.Azure
{
    /// <summary>
    /// A generic table data access object.
    /// </summary>
    /// <typeparam name="T">The type of table to handle.</typeparam>
    public class AppServiceTableDAO<T>
    {
        /// <summary>
        /// The database table to handle.
        /// </summary>
        public IMobileServiceSyncTable<T> TableLocal { get; set; }

        /// <summary>
        /// The database table to handle.
        /// </summary>
        public IMobileServiceTable<T> TableCloud { get; set; }

        /// <summary>
        /// Specifies whether the app stores data locally.
        /// </summary>
        private bool _supportsLocalStore { get; set; }

        /// <summary>
        /// Creates a new instance of the data access object.
        /// </summary>
        /// <param name="azureClient">The Azure App Service client.</param>
        public AppServiceTableDAO(LightBuzzMobileServiceClient azureClient)
        {
            _supportsLocalStore = azureClient.SupportsLocalStore;

            if (_supportsLocalStore)
                TableLocal = azureClient.GetSyncTable<T>();
            else
                TableCloud = azureClient.GetTable<T>();
        }

        /// <summary>
        /// Pulls the data from the remote Azure App Service and stores them into the local database.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="uniqueQueryId">The unique ID of the query.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns></returns>
        public async Task Pull(CancellationToken ct, string uniqueQueryId, Expression<Func<T, bool>> predicate)
        {
            if (_supportsLocalStore)
                await TableLocal.PullAsync(uniqueQueryId, TableLocal.Where(predicate), ct);
        }

        /// <summary>
        /// Performs a database Insert operation.
        /// </summary>
        /// <param name="objectToSave">The object to save.</param>
        /// <returns></returns>
        public async Task Insert(T objectToSave)
        {
            if (_supportsLocalStore)
                await TableLocal.InsertAsync(objectToSave);
            else
                await TableCloud.InsertAsync(objectToSave);
        }

        /// <summary>
        /// Performs a database Update operation.
        /// </summary>
        /// <param name="objectToUpdate">The object to update.</param>
        /// <returns></returns>
        public async Task Update(T objectToUpdate)
        {
            if (_supportsLocalStore)
                await TableLocal.UpdateAsync(objectToUpdate);
            else
                await TableCloud.UpdateAsync(objectToUpdate);
        }

        /// <summary>
        /// Performs a database Delete operation.
        /// </summary>
        /// <param name="objectToDelete">The object to delete.</param>
        /// <returns></returns>
        public async Task Delete(T objectToDelete)
        {
            if (_supportsLocalStore)
                await TableLocal.DeleteAsync(objectToDelete);
            else
                await TableCloud.DeleteAsync(objectToDelete);
        }

        /// <summary>
        /// Performs a database Get operation and returns all of the items included in the table.
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> FindAll()
        {
            if (_supportsLocalStore)
                return await TableLocal.ToListAsync();
            else
                return await TableCloud.ToListAsync();
        }

        /// <summary>
        /// Performs a database Get operation and returns all of the items that correspond to the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns></returns>
        public async Task<List<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            if (_supportsLocalStore)
                return await TableLocal.Where(predicate).ToListAsync();
            else
                return await TableCloud.Where(predicate).ToListAsync();
        }
    }
}
