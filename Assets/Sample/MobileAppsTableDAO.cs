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
        /// Determines whether the app will store data locally.
        /// </summary>
        public bool SupportsLocalStore { get; set; }

        /// <summary>
        /// Creates a new instance of the data access object.
        /// </summary>
        /// <param name="azureClient">The Azure App Service client.</param>
        protected MobileAppsTableDAO(MobileServiceClient azureClient)
        {
            if (SupportsLocalStore)
                TableLocal = azureClient.GetSyncTable<T>();
            else
                TableCloud = azureClient.GetTable<T>();
        }

        public static async Task<MobileAppsTableDAO<T>> Init(MobileServiceClient azureClient)
        {
            return await Init(azureClient, true);
        }

        public static async Task<MobileAppsTableDAO<T>> Init(MobileServiceClient azureClient, bool supportLocal)
        {
            MobileAppsTableDAO<T> dao = new MobileAppsTableDAO<T>(azureClient);

            if (supportLocal)
            {
                await LocalStore.Init(azureClient);
                await LocalStore.Sync();
            }

            return dao;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="uniqueQueryId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task Pull(CancellationToken ct, string uniqueQueryId, Expression<Func<T, bool>> predicate)
        {
            if (SupportsLocalStore)
                await TableLocal.PullAsync(uniqueQueryId, TableLocal.Where(predicate), ct);
        }

        /// <summary>
        /// Insert operation.
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
        /// 
        /// </summary>
        /// <param name="objectToUpdate"></param>
        /// <returns></returns>
        public async Task Update(T objectToUpdate)
        {
            if (SupportsLocalStore)
                await TableLocal.UpdateAsync(objectToUpdate);
            else
                await TableCloud.UpdateAsync(objectToUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectToDelete"></param>
        /// <returns></returns>
        public async Task Delete(T objectToDelete)
        {
            if (SupportsLocalStore)
                await TableLocal.DeleteAsync(objectToDelete);
            else
                await TableCloud.DeleteAsync(objectToDelete);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="predicate"></param>
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
