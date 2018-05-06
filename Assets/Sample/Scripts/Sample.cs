using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz
{
    public class Sample : MonoBehaviour
    {
        private MobileServiceClient azureClient;
        private MobileAppsTableDAO<TodoItem> todoTableDAO;

        [Header("Azure")]

        [SerializeField]
        [Tooltip("The Azure App Service URL")]
        private string mobileAppUri = "https://testtodolightbuzz.azurewebsites.net";

        [SerializeField]
        [Tooltip("Support local database")]
        private bool supportLocalDatabase = true;
                
        [Header("User Interface")]

        [SerializeField]
        private Text output;

        [SerializeField]
        private GameObject loading;

        [SerializeField]
        private Button buttonGet;

        [SerializeField]
        private Button buttonInsert;

        [SerializeField]
        private InputField inputInsert;

        [SerializeField]
        private Toggle toggleInsert;

        [SerializeField]
        private Button buttonDelete;

        [SerializeField]
        private Button buttonSync;

        private async void Start()
        {
            loading.SetActive(true);

            await Init();

            loading.SetActive(false);
        }

        public async void Get_Click()
        {
            loading.SetActive(true);

            await Get();

            loading.SetActive(false);
        }

        public async void Insert_Click()
        {
            loading.SetActive(true);

            await Insert();
            await Get();

            loading.SetActive(false);
        }

        public async void Delete_Click()
        {
            loading.SetActive(true);

            await Delete();
            await Get();

            loading.SetActive(false);
        }

        public async void Sync_Click()
        {
            loading.SetActive(true);

            await Sync();

            loading.SetActive(false);
        }

        private async Task Init()
        {
            azureClient = new MobileServiceClient(mobileAppUri, new LightBuzzHttpsHandler());
            todoTableDAO = new MobileAppsTableDAO<TodoItem>(azureClient, supportLocalDatabase);

            if (todoTableDAO.SupportsLocalStore)
            {
                await LocalStore.Init(azureClient);
                await LocalStore.Sync();
            }
        }

        private async Task Get()
        {
            List<TodoItem> list = await todoTableDAO.FindAll();

            StringBuilder contents = new StringBuilder();

            foreach (TodoItem item in list)
            {
                contents.AppendLine("ID: " + item.Id);
                contents.AppendLine("Text: " + item.Text);
                contents.AppendLine("Completed: " + item.Complete);
                contents.AppendLine();
            }

            output.text = contents.ToString();
        }

        private async Task Insert()
        {
            TodoItem item1 = new TodoItem
            {
                Text = inputInsert.text,
                Complete = toggleInsert.isOn
            };

            await todoTableDAO.Insert(item1);
        }

        private async Task Delete()
        {
            List<TodoItem> list = await todoTableDAO.FindAll();

            TodoItem last = list.LastOrDefault();

            await todoTableDAO.Delete(last);
        }

        private async Task Sync()
        {
            if (todoTableDAO != null && todoTableDAO.SupportsLocalStore)
                await LocalStore.Sync();
        }
    }
}
