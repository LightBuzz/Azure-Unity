using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UnityEngine;

namespace LightBuzz
{
    public class Sample : MonoBehaviour
    {
        private MobileServiceClient azureClient;
        private MobileAppsTableDAO<TodoItem> todoTableDAO;

        [SerializeField]
        private string mobileAppUri = "https://testtodolightbuzz.azurewebsites.net";

        public async Task Init()
        {
            azureClient = new MobileServiceClient(mobileAppUri, new LightBuzzHttpsHandler());
            todoTableDAO = await MobileAppsTableDAO<TodoItem>.Init(azureClient);
        }

        public async Task DeleteItem()
        {
            TodoItem lastNonCompleteItem = await GetLastNonCompleteItem();

            await todoTableDAO.Delete(lastNonCompleteItem);
        }

        public async Task InsertNewItem()
        {
            TodoItem item1 = new TodoItem { Text = "Item 1 " + LocalStore.TargetPlatform };
            await todoTableDAO.Insert(item1);
        }

        public async Task UpdateLastItem()
        {
            TodoItem lastItem = await GetLastItem();
            lastItem.Complete = true;
            lastItem.Text += " Updated " + LocalStore.TargetPlatform;

            await todoTableDAO.Update(lastItem);
        }

        public async Task ListCompletedItems()
        {
            List<TodoItem> listComplete = await todoTableDAO.FindAll(x => x.Complete);

            foreach (TodoItem item in listComplete)
            {
                Debug.Log($"{item.Id} - {item.Text} - {item.Complete}");
            }
        }

        public async Task<TodoItem> GetLastItem()
        {
            List<TodoItem> list = await todoTableDAO.FindAll();

            return list.LastOrDefault();
        }

        public async Task<TodoItem> GetLastNonCompleteItem()
        {
            List<TodoItem> list = await todoTableDAO.FindAll();

            return list.LastOrDefault(x => !x.Complete);
        }

        private async Task Sync()
        {
            await LocalStore.Sync();
        }
    }
}
