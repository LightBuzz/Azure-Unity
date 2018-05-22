using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightBuzz.Azure;
using Microsoft.WindowsAzure.MobileServices;

namespace Assets.LightBuzz.LightBuzz.Azure
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


    }
}
