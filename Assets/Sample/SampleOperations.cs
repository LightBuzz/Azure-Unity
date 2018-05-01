using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using UnityEngine;

namespace Assets.Sample
{
    public class SampleOperations : Sample
    {
        private IMobileServiceSyncTable<TodoItem> _tbl;
        public async void EasyTablesTest()
        {
            //ClearOutput();
            Debug.Log("-- Testing Easy Tables --");

            await InitLocalStoreAsync();

            try
            {
                var listAll = await ListAll();

                await InsertItems();

                listAll = await ListAll();
                await UpdateItem(listAll);

                await ListComplete();

                listAll = await ListAll();
                await DeleteItem(listAll);

                await PushAsync();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR");
                Debug.Log("ERROR");
                Debug.Log(e.ToString());
            }

            Debug.Log("-- Test Complete --");

        }

        private async Task DeleteAll(List<TodoItem> listAll)
        {
            Debug.Log("Deleting all items");
            foreach (TodoItem item in listAll)
            {
                await _tbl.DeleteAsync(item);
            }
        }

        private async Task InsertItems()
        {
            Debug.Log("Inserting new items");
//#if UNITY_ANDROID
//                await tbl.InsertAsync(new TodoItem { Text = "Item Android" });
//#else
//        await tbl.InsertAsync(new TodoItem { Text = "Item Windows" });
//#endif
            await _tbl.InsertAsync(new TodoItem { Text = "Item Editor" });
            //await tbl.InsertAsync(new TodoItem { Text = "Second Item Editor" });
            //await tbl.InsertAsync(new TodoItem { Text = "Third Item Editor" });
            //await tbl.InsertAsync(new TodoItem { Text = "Fourth Item Editor" });
        }

        private async Task UpdateItem(List<TodoItem> listAll)
        {
            Debug.Log("Updating last item");
            int idx = listAll.Count - 1;
            listAll[idx].Complete = true;
//#if UNITY_ANDROID
//        listAll[idx].Text = listAll[idx].Text + " Updated Android";
//#else
//        listAll[idx].Text = listAll[idx].Text + " Updated Windows";
//#endif
            listAll[idx].Text = listAll[idx].Text + " Updated Editor";
            await _tbl.UpdateAsync(listAll[idx]);
        }

        private async Task DeleteItem(List<TodoItem> listAll)
        {
            Debug.Log("Deleting first item");
            await _tbl.DeleteAsync(listAll[0]);
        }

        private async Task ListComplete()
        {
            Debug.Log("Getting 5 complete items");
            List<TodoItem> listComplete = await _tbl.Where(x => x.Complete).Take(5).ToListAsync();
            foreach (TodoItem item in listComplete)
            {
                Debug.Log($"{item.Id} - {item.Text} - {item.Complete}");
            }
        }
    
        private async Task<List<TodoItem>> ListAll()
        {
            Debug.Log("Getting all items");
            List<TodoItem> listAll = await _tbl.ToListAsync();
            Debug.Log("Total items no " + listAll.Count);
            return listAll;
        }

        private async Task InitLocalStoreAsync()
        {
            if (!AzureClient.SyncContext.IsInitialized)
            {
                try
                {
                    Debug.Log("-- Check if local db exists --");
                    Debug.Log("Path: "+Application.persistentDataPath);
                    Debug.Log(Application.persistentDataPath);
                    string databasePath = Application.persistentDataPath + "\\localstore.db";
                    if (!File.Exists(databasePath))
                    {
                        Debug.Log("-- Creating local db --");
                        File.Create(databasePath);
                        Debug.Log("-- Local db created --");
                    }

                    Debug.Log("-- Opening local db --");
                    var store = new MobileServiceSQLiteStore(databasePath);

                    Debug.Log("-- Create table --");
                    store.DefineTable<TodoItem>();
                    await AzureClient.SyncContext.InitializeAsync(store);

                    Debug.Log("-- Getting table --");
                    _tbl = AzureClient.GetSyncTable<TodoItem>();
                }
                catch (Exception e)
                {
                    Debug.Log("ERROR");
                    Debug.Log(e.ToString());
                    Debug.Log("ERROR");
                    Debug.Log(e.Message);
                    Debug.Log(e.Source);
                    Debug.Log(e.StackTrace);
                    Debug.Log("THIS IS TO STRING:-------------------------------------------");
                    Debug.Log(e.ToString());
                }
            }
            Debug.Log("-- Sync --");
            await SyncAsync();
        }

        private async Task SyncAsync()
        {
            try
            {
                await PushAsync();
                await PullAsync();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR");
                Debug.Log(e.ToString());
                Debug.Log("ERROR");
                Debug.Log(e.Message);
                Debug.Log(e.Source);
                Debug.Log(e.StackTrace);
                Debug.Log("THIS IS TO STRING:-------------------------------------------");
                Debug.Log(e.ToString());
            }
        }

        private async Task PullAsync()
        {
            Debug.Log("-- Pull --");
            await _tbl.PullAsync("todoItems", _tbl.CreateQuery());
        }

        private async Task PushAsync()
        {
            Debug.Log("-- Push --");
            await AzureClient.SyncContext.PushAsync();
        }

    }
}
