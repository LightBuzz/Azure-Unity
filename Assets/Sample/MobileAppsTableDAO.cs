using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace LightBuzz
{
    public class MobileAppsTableDAO<T>
    {
        public IMobileServiceSyncTable<T> Table { get; set; }

        public MobileAppsTableDAO(MobileServiceClient azureClient)
        {
            Table = azureClient.GetSyncTable<T>();
        }

        public async Task LocalPullAsync(CancellationToken ct, string uniqueQueryId, Expression<Func<T, bool>> predicate)
        {
            await Table.PullAsync(uniqueQueryId, Table.Where(predicate), ct);
        }

        public async Task Insert(T objectToSave)
        {
            await Table.InsertAsync(objectToSave);
        }

        public async Task Update(T objectToUpdate)
        {
            await Table.UpdateAsync(objectToUpdate);
        }

        public async Task Delete(T objectToDelete)
        {
            await Table.DeleteAsync(objectToDelete);
        }

        public async Task<List<T>> FindAll()
        {
            List<T> res = await Table.ToListAsync();
            return res;
        }

        public async Task<List<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            List<T> res = await Table.Where(predicate).ToListAsync();
            return res;
        }
    }
}
