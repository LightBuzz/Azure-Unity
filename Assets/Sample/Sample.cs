using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Assets.Sample
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] protected string MobileAppUri = "https://testtodolightbuzz.azurewebsites.net";
        protected MobileServiceClient AzureClient;
        private IMobileServiceSyncTable<TodoItem> _todoItemsTable;

        // Use this for initialization
        void Start()
        {
            var handler = new LightBuzzHttpsHandler() { AutomaticDecompression = DecompressionMethods.Deflate };
            AzureClient = new MobileServiceClient(MobileAppUri, handler);
        }

        public async void RunSample()
        {
            Debug.Log("-- Start Sample --");

            //Initialize local db
            await InitLocalStore();

            MobileAppsTableDAO<TodoItem> todoItemDAO = new MobileAppsTableDAO<TodoItem>(AzureClient);

            //Insert a new item
            TodoItem item1 = new TodoItem {Text = "Item 1 " + PlatformExtensions.OperatingSystem};
            await todoItemDAO.Insert(item1);
            TodoItem item2 = new TodoItem { Text = "Item 2 " + PlatformExtensions.OperatingSystem };
            await todoItemDAO.Insert(item2);
            TodoItem item3 = new TodoItem { Text = "Item 3 " + PlatformExtensions.OperatingSystem };
            await todoItemDAO.Insert(item3);

            //Update last item
            TodoItem lastItem = await GetLastItem();
            lastItem.Complete = true;
            lastItem.Text += " Updated " + PlatformExtensions.OperatingSystem;
            await todoItemDAO.Update(lastItem);

            //Print list of complete items
            await ListComplete();

            //Delete last non complete item
            TodoItem lastNonCompleteItem = await GetLastNonCompleteItem();
            await todoItemDAO.Delete(lastNonCompleteItem);

            //Push changes to server
            await PushAsync();

            Debug.Log("-- Sample Complete --");

        }

        

        private async Task ListComplete()
        {
            Debug.Log("Getting completed items");
            List<TodoItem> listComplete = await _todoItemsTable.Where(x => x.Complete).Take(5).ToListAsync();
            foreach (TodoItem item in listComplete)
            {
                Debug.Log($"{item.Id} - {item.Text} - {item.Complete}");
            }
        }

        private async Task<List<TodoItem>> ListAll()
        {
            Debug.Log("Getting all items");
            List<TodoItem> listAll = await _todoItemsTable.ToListAsync();
            Debug.Log("Total items no " + listAll.Count);
            return listAll;
        }

        private async Task<TodoItem> GetLastItem()
        {
            List<TodoItem> list = await ListAll();
            return list.LastOrDefault();
        }

        private async Task<TodoItem> GetLastNonCompleteItem()
        {
            List<TodoItem> list = await ListAll();
            return list.LastOrDefault(x=>!x.Complete);
        }

        private async Task InitLocalStore()
        {
            if (!AzureClient.SyncContext.IsInitialized)
            {
                try
                {
                    Debug.Log("-- Check if local db exists --");
                    Debug.Log("Path: " + Application.persistentDataPath);

                    string databasePath = CreateLocalDb();

                    Debug.Log("-- Opening local db --");
                    var store = new MobileServiceSQLiteStore(databasePath);

                    Debug.Log("-- Create table --");
                    store.DefineTable<TodoItem>();
                    await AzureClient.SyncContext.InitializeAsync(store);

                    GetTable();
                }
                catch (Exception e)
                {
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

        private void GetTable()
        {
            Debug.Log("-- Getting table --");
            _todoItemsTable = AzureClient.GetSyncTable<TodoItem>();
        }

        private static string CreateLocalDb()
        {
            string databasePath = Application.persistentDataPath + "\\localstore.db";
            if (!File.Exists(databasePath))
            {
                Debug.Log("-- Creating local db --");
                File.Create(databasePath);
            }

            return databasePath;
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
            await _todoItemsTable.PullAsync("todoItems", _todoItemsTable.CreateQuery());
        }

        private async Task PushAsync()
        {
            Debug.Log("-- Push --");
            await AzureClient.SyncContext.PushAsync();
        }

    }
}
