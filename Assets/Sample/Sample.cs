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

namespace LightBuzz
{
    public class Sample : MonoBehaviour
    {
        [SerializeField]
        private string MobileAppUri = "https://testtodolightbuzz.azurewebsites.net";

        private MobileServiceClient _azureClient;
        private IMobileServiceSyncTable<TodoItem> _todoItemsTable;

        private string LocalDatabaseName
        {
            get
            {
                return "localstore.db";
            }
        }

        private string LocalDatabasePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, LocalDatabaseName);
            }
        }

        private void Start()
        {
            _azureClient = new MobileServiceClient(MobileAppUri, new LightBuzzHttpsHandler());
        }

        public async void RunSample()
        {
            Debug.Log("-- Sample started --");

            // Initialize the local database
            await InitLocalStore();
            await SyncAsync();

            MobileAppsTableDAO<TodoItem> todoItemDAO = new MobileAppsTableDAO<TodoItem>(_azureClient);

            // Insert a new item
            TodoItem item1 = new TodoItem { Text = "Item 1 " + PlatformTools.TargetPlatform };
            await todoItemDAO.Insert(item1);

            TodoItem item2 = new TodoItem { Text = "Item 2 " + PlatformTools.TargetPlatform };
            await todoItemDAO.Insert(item2);

            TodoItem item3 = new TodoItem { Text = "Item 3 " + PlatformTools.TargetPlatform };
            await todoItemDAO.Insert(item3);

            // Update the last item
            TodoItem lastItem = await GetLastItem();
            lastItem.Complete = true;
            lastItem.Text += " Updated " + PlatformTools.TargetPlatform;

            await todoItemDAO.Update(lastItem);

            // Print a list of complete items
            await ListComplete();

            //Delete last non complete item
            TodoItem lastNonCompleteItem = await GetLastNonCompleteItem();
            await todoItemDAO.Delete(lastNonCompleteItem);

            //Push changes to server
            await PushAsync();

            Debug.Log("-- Sample completed --");
        }

        private async Task ListComplete()
        {
            Debug.Log("Getting completed items...");

            List<TodoItem> listComplete = await _todoItemsTable.Where(x => x.Complete).Take(5).ToListAsync();

            foreach (TodoItem item in listComplete)
            {
                Debug.Log($"{item.Id} - {item.Text} - {item.Complete}");
            }
        }

        private async Task<List<TodoItem>> ListAll()
        {
            Debug.Log("Getting all items");

            List<TodoItem> list = await _todoItemsTable.ToListAsync();

            Debug.Log("Total items no " + list.Count);

            return list;
        }

        private async Task<TodoItem> GetLastItem()
        {
            List<TodoItem> list = await ListAll();

            return list.LastOrDefault();
        }

        private async Task<TodoItem> GetLastNonCompleteItem()
        {
            List<TodoItem> list = await ListAll();

            return list.LastOrDefault(x => !x.Complete);
        }

        private async Task InitLocalStore()
        {
            if (!_azureClient.SyncContext.IsInitialized)
            {
                try
                {
                    Debug.Log("-- Check if local db exists --");
                    Debug.Log("Path: " + LocalDatabasePath);

                    if (!File.Exists(LocalDatabasePath))
                    {
                        Debug.Log("-- Creating local db --");

                        PlatformTools.CopyDatabase(LocalDatabaseName, LocalDatabasePath);
                    }

                    Debug.Log("-- Opening local db --");

                    MobileServiceSQLiteStore store = new MobileServiceSQLiteStore(LocalDatabasePath);

                    Debug.Log("-- Create table --");

                    store.DefineTable<TodoItem>();
                    await _azureClient.SyncContext.InitializeAsync(store);

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
        }

        private void GetTable()
        {
            Debug.Log("-- Getting table --");

            _todoItemsTable = _azureClient.GetSyncTable<TodoItem>();
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

            await _azureClient.SyncContext.PushAsync();
        }

    }
}
