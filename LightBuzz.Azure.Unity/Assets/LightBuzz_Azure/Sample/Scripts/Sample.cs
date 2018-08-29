//
// Copyright (c) LightBuzz Software.
// All rights reserved.
//
// http://lightbuzz.com
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
// COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
// OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
// AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//

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
    private AppServiceTableDAO<TodoItem> todoTableDAO;

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
        if (buttonSync != null)
        {
            buttonSync.gameObject.SetActive(supportLocalDatabase);
        }

        if (textSync != null)
        {
            textSync.gameObject.SetActive(supportLocalDatabase);
        }
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

        todoTableDAO = new AppServiceTableDAO<TodoItem>(azureClient);
    }

    private async Task Get()
    {
        StringBuilder contents = new StringBuilder();
        List<TodoItem> list = new List<TodoItem>();

        await Task.Run(async () =>
        {
            list = await todoTableDAO.FindAll();
        });

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
        await azureClient.Sync();

    }
}