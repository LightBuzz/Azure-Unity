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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightBuzz.Azure
{
    /// <summary>
    /// Represents an Azure Service Client that has no Local Store support.
    /// </summary>
    public class LightBuzzMobileServiceClientCloudOnly : LightBuzzMobileServiceClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LightBuzzMobileServiceClientCloudOnly"/>.
        /// </summary>
        /// <param name="mobileAppUri">The Azure service URI.</param>
        public LightBuzzMobileServiceClientCloudOnly(string mobileAppUri) : base(mobileAppUri, false)
        {
        }

        private LightBuzzMobileServiceClientCloudOnly(string mobileAppUri, bool supportLocal) : base(mobileAppUri, supportLocal)
        {
        }

        /// <summary>
        /// Defines the database tables in managed code.
        /// </summary>
        protected override void DefineTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pulls the remote data from the server.
        /// </summary>
        /// <param name="ct">The Cancellation Token.</param>
        /// <returns></returns>
        public override Task Pull(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
