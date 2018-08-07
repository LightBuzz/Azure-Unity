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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LightBuzz.Azure
{
    /// <summary>
    /// A Unity-ready secure HTTPS handler.
    /// </summary>
    public class LightBuzzHttpsHandler : HttpClientHandler
    {
        /// <summary>
        /// The response from the server.
        /// </summary>
        private HttpResponseMessage _result = new HttpResponseMessage();

        /// <summary>
        /// The authorization token for the request.
        /// </summary>
        private string _authorizationToken = string.Empty;

        /// <summary>
        /// The Content Type header type (default: application/json).
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The ZUMO API version number.
        /// </summary>
        public string ZumoApiVersion { get; set; }

        /// <summary>
        /// The encoding of the response message.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Creates a new LightBuzz secure HTTPS handler.
        /// </summary>
        public LightBuzzHttpsHandler()
        {
            AutomaticDecompression = DecompressionMethods.Deflate;
            ContentType = "application/json";
            ZumoApiVersion = "2.0.0";
            Encoding = Encoding.UTF8;
        }

#if !UNITY_2018_2

        /// <summary>
        /// A Unity-ready implementation of a secure HTTPS method to send the request.
        /// </summary>
        /// <param name="request">The request to send to server.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the server.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string contentArray = request.Content != null ? await request.Content.ReadAsStringAsync() : null;

            HttpRequestHeaders headers = request.Headers;
            IEnumerable<string> auth;
            if (headers != null && headers.TryGetValues("X-ZUMO-AUTH", out auth))
            {
                _authorizationToken = headers.GetValues("X-ZUMO-AUTH").FirstOrDefault();
            }

            SendHttpWebRequest(request, contentArray);

            return _result;
        }

        /// <summary>
        /// Sends an Http Web Request.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="contentArray">The request content.</param>
        /// <returns></returns>
        private void SendHttpWebRequest(HttpRequestMessage request, string contentArray)
        {
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(request.RequestUri.AbsoluteUri);

            client.Method = request.Method.ToString();
            client.KeepAlive = true;
            client.ContentType = ContentType;

            if (!WebHeaderCollection.IsRestricted("Content-Type"))
            {
                client.Headers.Add("Content-Type", ContentType);
            }

            if (!WebHeaderCollection.IsRestricted("ZUMO-API-VERSION"))
            {
                client.Headers.Add("ZUMO-API-VERSION", ZumoApiVersion);
            }

            if (!string.IsNullOrEmpty(_authorizationToken))
            {
                if (!WebHeaderCollection.IsRestricted("X-ZUMO-AUTH"))
                {
                    client.Headers.Add("X-ZUMO-AUTH", _authorizationToken);
                }
            }

#if !UNITY_WSA
            ServicePointManager.ServerCertificateValidationCallback = LightBuzzCertificateValidation.CertificateValidationCallback;
#endif

            if (contentArray != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(client.GetRequestStream()))
                {
                    streamWriter.Write(contentArray);
                }
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)client.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();

                        _result.StatusCode = response.StatusCode;
                        _result.ReasonPhrase = _result.StatusCode.ToString();
                        _result.Content = new StringContent(data, Encoding, ContentType);
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Response==null)
                {
                    throw webException;
                }
                using (HttpWebResponse response = (HttpWebResponse)webException.Response)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();

                        _result.StatusCode = response.StatusCode;
                        _result.ReasonPhrase = _result.StatusCode.ToString();
                        _result.Content = new StringContent(data, Encoding, ContentType);
                    }
                }
            }
        }

        /*/// <summary>
        /// A Unity-ready implementation of a secure HTTPS method to send the request.
        /// </summary>
        /// <param name="request">The request to send to server.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the server.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            byte[] contentArray = request.Content != null ? await request.Content.ReadAsByteArrayAsync() : null;

            HttpRequestHeaders headers = request.Headers;
            IEnumerable<string> auth;
            if (headers != null && headers.TryGetValues("X-ZUMO-AUTH", out auth))
            {
                _authorizationToken = headers.GetValues("X-ZUMO-AUTH").FirstOrDefault();
            }
            IEnumerator enumerator = SendUnityRequest(request.RequestUri.AbsoluteUri, contentArray, request.Method.ToString());

            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                }
            }

            return _result;
        }*/

        /// <summary>
        /// Sends a Unity Web Request.
        /// </summary>
        /// <param name="url">The request absolute uri.</param>
        /// <param name="contentArray">The request content.</param>
        /// <param name="method">The request method.</param>
        /// <returns>An IEnumerator with the response text.</returns>
        IEnumerator SendUnityRequest(string url, byte[] contentArray, string method)
        {
            UnityWebRequest uwr = new UnityWebRequest(url, method);

            if (contentArray != null)
            {
                uwr.uploadHandler = new UploadHandlerRaw(contentArray);
            }

            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", ContentType);
            uwr.SetRequestHeader("ZUMO-API-VERSION", ZumoApiVersion);
            if (!string.IsNullOrEmpty(_authorizationToken))
            {
                uwr.SetRequestHeader("X-ZUMO-AUTH", _authorizationToken);
            }

            // Send the request then wait until it returns.
            yield return uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning("Error while sending: " + uwr.error);
                _result.StatusCode = (HttpStatusCode)uwr.responseCode;
                _result.ReasonPhrase = _result.StatusCode.ToString();
                _result.Content = new StringContent(uwr.downloadHandler.text, Encoding.UTF8, ContentType);

                yield return uwr.error;
            }
            else
            {
                _result.StatusCode = (HttpStatusCode)uwr.responseCode;
                _result.ReasonPhrase = _result.StatusCode.ToString();
                _result.Content = new StringContent(uwr.downloadHandler.text, Encoding.UTF8, ContentType);

                yield return uwr.downloadHandler.text;
            }
        }

#endif

    }
}
