using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace LightBuzz.Azure
{
    public class LightBuzzMobileServiceClient : MobileServiceClient
    {
        /// <summary>
        /// Specifies whether the app will store data locally.
        /// </summary>
        public bool SupportsLocalStore { get; set; }

#if !UNITY_WSA || UNITY_EDITOR

        public LightBuzzMobileServiceClient(string mobileAppUri,bool supportLocal) : base(mobileAppUri, new LightBuzzHttpsHandler())
        {
            SupportsLocalStore = supportLocal;
        }
#else

        public LightBuzzMobileServiceClient(string mobileAppUri,bool supportLocal) : base(mobileAppUri)
        {
            SupportsLocalStore = supportLocal;
        }

#endif

        public async Task InitializeLocalStore()
        {
            await InitStore();
            await SyncStore();
        }

        public async Task InitStore()
        {
            if (SupportsLocalStore)
            {
                await LocalStore.Init(this);
            }
        }

        public async Task SyncStore()
        {
            if (SupportsLocalStore)
            {
                await LocalStore.Sync();
            }
        }
    }
}
