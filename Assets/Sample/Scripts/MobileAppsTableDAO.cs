using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace LightBuzz
{
    /// <summary>
    /// A generic table data access object.
    /// </summary>
    /// <typeparam name="T">The type of table to handle.</typeparam>
    public class MobileAppsTableDAO<T>
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
        /// Specifies whether the app will store data locally.
        /// </summary>
        public bool SupportsLocalStore { get; set; }

        /// <summary>
        /// Creates a new instance of the data access object.
        /// </summary>
        /// <param name="azureClient">The Azure App Service client.</param>
        public MobileAppsTableDAO(MobileServiceClient azureClient) : this(azureClient, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the data access object.
        /// </summary>
        /// <param name="azureClient">The Azure App Service client.</param>
        /// <param name="supportLocal">Specifies whether the app will store data locally.</param>
        public MobileAppsTableDAO(MobileServiceClient azureClient, bool supportLocal)
        {
            SupportsLocalStore = supportLocal;

            if (SupportsLocalStore)
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
            if (SupportsLocalStore)
                await TableLocal.PullAsync(uniqueQueryId, TableLocal.Where(predicate), ct);
        }

        /// <summary>
        /// Performs a database Insert operation.
        /// </summary>
        /// <param name="objectToSave">The object to save.</param>
        /// <returns></returns>
        public async Task Insert(T objectToSave)
        {
            if (SupportsLocalStore)
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
            if (SupportsLocalStore)
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
            if (SupportsLocalStore)
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
            if (SupportsLocalStore)
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
            if (SupportsLocalStore)
                return await TableLocal.Where(predicate).ToListAsync();
            else
                return await TableCloud.Where(predicate).ToListAsync();
        }
    }
}
