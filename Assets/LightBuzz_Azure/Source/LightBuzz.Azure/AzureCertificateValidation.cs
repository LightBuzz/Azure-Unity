﻿//
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

#if !UNITY_WSA || UNITY_EDITOR
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LightBuzz.Azure
{
    /// <summary>
    /// Validates a remote SSL certificate for Azure.
    /// </summary>
    public class AzureCertificateValidation : ICertificateValidator
    {
        /// <summary>
        /// The information for the client's proxy. If no proxy is used, should be null or empty.
        /// </summary>
        public string ProxyInfo { get; set; }

        /// <summary>
        /// Creates a new AzureCertificateValidation.
        /// </summary>
        /// <param name="proxyInfo">The information for the client's proxy. If no proxy is used, should be null or empty.</param>
        public AzureCertificateValidation(string proxyInfo)
        {
            ProxyInfo = proxyInfo;
        }

        /// <summary>
        /// Determines whether the specified SSL certificate is valid. Implemented for Azure certificates.
        /// </summary>
        /// <param name="sender">The object raising the callback.</param>
        /// <param name="certificate">The certificate to validate.</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns>True if the certificate is valid. False otherwise.</returns>
        public bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isValidCertificate = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (var st in chain.ChainStatus)
                {
                    if (st.Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                    if (!chain.Build((X509Certificate2)certificate))
                    {
                        isValidCertificate = false;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(ProxyInfo))
            {
                if (!certificate.Subject.Contains("CN=*.azurewebsites.net") && !certificate.Subject.Contains("CN=*.blob.core.windows.net"))
                {
                    return false;
                }
            }

            return isValidCertificate;
        }
    }
}
#endif