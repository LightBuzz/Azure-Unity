using LightBuzz.Azure;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private LightBuzzMobileServiceClient azureClient;
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

    [SerializeField]
    private Text textSync;

    private async void Start()
    {
        loading.SetActive(true);

        await Init();

        loading.SetActive(false);
    }

    private void OnValidate()
    {
        buttonSync.gameObject.SetActive(supportLocalDatabase);
        textSync.gameObject.SetActive(supportLocalDatabase);
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
        azureClient = new SampleMobileClient(mobileAppUri, supportLocalDatabase);
        await azureClient.InitializeLocalStore();

        todoTableDAO = new MobileAppsTableDAO<TodoItem>(azureClient);
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
        await azureClient.SyncStore();

    }
}