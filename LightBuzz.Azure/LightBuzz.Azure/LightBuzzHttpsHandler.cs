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

#if !UNITY_WSA

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightBuzz.Azure
{
    /// <summary>
    /// A Unity-ready secure HTTPS handler.
    /// </summary>
    public class LightBuzzHttpsHandler : HttpClientHandler
    {
        #region Constants

        private const string DefaultContentType = "application/json";
	    private const string DefaultZumoApiVersion = "2.0.0";
	    private const int DefaultTimeout = 60000;
		private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private readonly ICertificateValidator Validator;


        #endregion

        #region Members

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
		/// The time-out value for the request in milliseconds. Default value is 60000.
		/// </summary>
		public int RequestTimeout { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new LightBuzz secure HTTPS handler.
		/// </summary>
		public LightBuzzHttpsHandler() :
		    this(DefaultContentType, DefaultZumoApiVersion, DefaultEncoding, DefaultTimeout, new AzureCertificateValidation(string.Empty))
        {
        }

        /// <summary>
	    /// Creates a new LightBuzz secure HTTPS handler with the specified parameters.
	    /// </summary>
	    /// <param name="requestTimeout">The request timeout value in milliseconds.</param>
	    public LightBuzzHttpsHandler(int requestTimeout) :
            this(DefaultContentType, DefaultZumoApiVersion, DefaultEncoding, requestTimeout, new AzureCertificateValidation(string.Empty))
        {
        }

        /// <summary>
        /// Creates a new LightBuzz secure HTTPS handler with the specified parameters.
        /// </summary>
        /// <param name="requestTimeout">The request timeout value in milliseconds.</param>
        /// <param name="validator">The certificate validator</param>
        public LightBuzzHttpsHandler(int requestTimeout, ICertificateValidator validator) :
            this(DefaultContentType, DefaultZumoApiVersion, DefaultEncoding, requestTimeout, validator)
        {
        }

        /// <summary>
        /// Creates a new LightBuzz secure HTTPS handler with the specified parameters.
        /// </summary>
        /// <param name="validator">The certificate validator</param>
        public LightBuzzHttpsHandler(ICertificateValidator validator) :
            this(DefaultContentType, DefaultZumoApiVersion, DefaultEncoding, DefaultTimeout, validator)
        {
        }

        /// <summary>
        /// Creates a new LightBuzz secure HTTPS handler with the specified parameters.
        /// </summary>
        /// <param name="contentType">The Content Type header type.</param>
        /// <param name="zumoApiVersion">The ZUMO API version number.</param>
        /// <param name="encoding">The encoding of the response message.</param>
        /// <param name="requestTimeout">The request timeout value in milliseconds.</param>
        public LightBuzzHttpsHandler(string contentType, string zumoApiVersion, Encoding encoding, int requestTimeout) :
            this(contentType, zumoApiVersion, encoding, requestTimeout, new AzureCertificateValidation(string.Empty))
        {
        }

        /// <summary>
        /// Creates a new LightBuzz secure HTTPS handler with the specified parameters.
        /// </summary>
        /// <param name="contentType">The Content Type header type.</param>
        /// <param name="zumoApiVersion">The ZUMO API version number.</param>
        /// <param name="encoding">The encoding of the response message.</param>
        /// <param name="requestTimeout">The request timeout value in milliseconds.</param>
        /// <param name="validator">The certificate validator</param>
        public LightBuzzHttpsHandler(string contentType, string zumoApiVersion, Encoding encoding, int requestTimeout, ICertificateValidator validator)
        {
            AutomaticDecompression = DecompressionMethods.Deflate;
            ContentType = contentType;
            ZumoApiVersion = zumoApiVersion;
            Encoding = encoding;
            RequestTimeout = requestTimeout;
            Validator = validator;
        }

		#endregion

		#region Methods

		/// <summary>
		/// A Unity-ready implementation of a secure HTTPS method to send the request.
		/// </summary>
		/// <param name="request">The request to send to server.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The response from the server.</returns>
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(request.RequestUri.AbsoluteUri);

            client.Method = request.Method.ToString();
            client.Timeout = RequestTimeout;
            client.KeepAlive = true;
            client.ContentType = ContentType;

            HttpRequestHeaders headers = request.Headers;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!WebHeaderCollection.IsRestricted(header.Key) && header.Value != null && header.Value.Count() != 0)
                    {
                        client.Headers.Add(header.Key, header.Value.FirstOrDefault());
                    }
                }
            }

            ServicePointManager.ServerCertificateValidationCallback = Validator.CertificateValidationCallback;

            string contentArray = request.Content != null ? await request.Content.ReadAsStringAsync() : null;
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

                        result.StatusCode = response.StatusCode;
                        result.ReasonPhrase = result.StatusCode.ToString();
                        result.Content = new StringContent(data, Encoding, ContentType);
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                {
                    throw webException;
                }

                using (HttpWebResponse response = (HttpWebResponse)webException.Response)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();

                        result.StatusCode = response.StatusCode;
                        result.ReasonPhrase = result.StatusCode.ToString();
                        result.Content = new StringContent(data, Encoding, ContentType);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}

#endif