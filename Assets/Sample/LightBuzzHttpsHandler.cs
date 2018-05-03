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
    public class LightBuzzHttpsHandler : HttpClientHandler
    {
        private HttpResponseMessage _result = new HttpResponseMessage();

        public LightBuzzHttpsHandler()
        {
            AutomaticDecompression = DecompressionMethods.Deflate;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            byte[] jsonArray = null;
            if (request.Content != null)
            {
                jsonArray = await request.Content.ReadAsByteArrayAsync();
            }
            IEnumerator e = SendUnityRequest(request.RequestUri.AbsoluteUri, jsonArray, request.Method.ToString());

            if (e != null)
            {
                while (e.MoveNext())
                    if (e.Current != null)
                        Debug.Log("e.current " + e.Current as string);
            }
            return _result;
        }

        IEnumerator SendUnityRequest(string url, byte[] jsonArray, string method)
        {
            var uwr = new UnityWebRequest(url, method);
            if (jsonArray != null)
            {
                uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonArray);
            }
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("ZUMO-API-VERSION", "2.0.0");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            while (!uwr.isDone)
                yield return null;

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
                yield return uwr.error;
            }
            else
            {
                _result.StatusCode = (HttpStatusCode)uwr.responseCode;
                _result.ReasonPhrase = _result.StatusCode.ToString();
                _result.Content = new StringContent(uwr.downloadHandler.text, Encoding.UTF8, "application/json");
                yield return uwr.downloadHandler.text;
            }
        }
    }
}
