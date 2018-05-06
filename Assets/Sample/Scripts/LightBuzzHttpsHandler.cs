using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LightBuzz
{
    /// <summary>
    /// A Unity-ready secure HTTPS handler.
    /// </summary>
    public class LightBuzzHttpsHandler : HttpClientHandler
    { 
        private HttpResponseMessage _result = new HttpResponseMessage();

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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            byte[] jsonArray = request.Content != null ? await request.Content.ReadAsByteArrayAsync() : null;

            IEnumerator enumerator = SendUnityRequest(request.RequestUri.AbsoluteUri, jsonArray, request.Method.ToString());

            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                }
            }

            return _result;
        }

        IEnumerator SendUnityRequest(string url, byte[] jsonArray, string method)
        {
            UnityWebRequest uwr = new UnityWebRequest(url, method);

            if (jsonArray != null)
            {
                uwr.uploadHandler = new UploadHandlerRaw(jsonArray);
            }

            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", ContentType);
            uwr.SetRequestHeader("ZUMO-API-VERSION", ZumoApiVersion);

            // Send the request then wait until it returns.
            yield return uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning("Error while sending: " + uwr.error);

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
    }
}
