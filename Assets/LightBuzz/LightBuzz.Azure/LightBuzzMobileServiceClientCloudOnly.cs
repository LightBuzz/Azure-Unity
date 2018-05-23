using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBuzz.Azure
{
    public class LightBuzzMobileServiceClientCloudOnly : LightBuzzMobileServiceClient
    {
        public LightBuzzMobileServiceClientCloudOnly(string mobileAppUri) : base(mobileAppUri, false)
        {
        }

        private LightBuzzMobileServiceClientCloudOnly(string mobileAppUri, bool supportLocal) : base(mobileAppUri, supportLocal)
        {
        }

        protected override void DefineTables()
        {
            throw new NotImplementedException();
        }

        public override Task Pull()
        {
            throw new NotImplementedException();
        }
    }
}
