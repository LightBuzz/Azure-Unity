using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightBuzz.Azure;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

class SampleMobileClient : LightBuzzMobileServiceClient
{
    public SampleMobileClient(string mobileAppUri, bool supportLocal) : base(mobileAppUri, supportLocal)
    {
    }

    protected override void DefineTables()
    {
        LocalStore.DefineTable<TodoItem>();
    }

    public override async Task Pull()
    {
        MobileAppsTableDAO<TodoItem> todoTableDao = new MobileAppsTableDAO<TodoItem>(this);
        await todoTableDao.Pull(new CancellationToken(), "TodoItems", x => x.Id != null);
    }
}
