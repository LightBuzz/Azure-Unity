using LightBuzz.Azure;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
        await Get();

        loading.SetActive(false);
    }

    private async Task Init()
    {
#if !UNITY_WSA || UNITY_EDITOR
        azureClient = new MobileServiceClient(mobileAppUri, new LightBuzzHttpsHandler());
#else
            azureClient = new MobileServiceClient(mobileAppUri);
#endif
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
        TodoItem item = new TodoItem
        {
            Text = inputInsert.text,
            Complete = toggleInsert.isOn
        };

        await todoTableDAO.Insert(item);
    }

    private async Task Delete()
    {
        List<TodoItem> list = await todoTableDAO.FindAll();

        TodoItem item = list.LastOrDefault();

        if (item != null)
        {
            await todoTableDAO.Delete(item);
        }
    }

    private async Task Sync()
    {
        if (todoTableDAO.SupportsLocalStore)
        {
            await LocalStore.Sync();
        }
    }
}