using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Sample
{
    class LightBuzzHttpsHandler : HttpClientHandler
    {
        private HttpResponseMessage _result = new HttpResponseMessage();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerator e = null;
            byte[] jsonArray;
            switch (request.Method.ToString())
            {
                case "GET":
                    e = SendUnityRequest(request.RequestUri.AbsoluteUri, null, "GET");
                    break;
                case "POST":
                    jsonArray = await request.Content.ReadAsByteArrayAsync();
                    e = SendUnityRequest(request.RequestUri.AbsoluteUri, jsonArray, "POST");
                    break;
                case "PATCH":
                    jsonArray = await request.Content.ReadAsByteArrayAsync();
                    e = SendUnityRequest(request.RequestUri.AbsoluteUri, jsonArray, "PATCH");
                    break;
                case "DELETE":
                    e = SendUnityRequest(request.RequestUri.AbsoluteUri, null, "DELETE");
                    break;
                default:
                    if (request.RequestUri.AbsoluteUri.Contains("https"))
                    {
                        UriBuilder builder = new UriBuilder(request.RequestUri);
                        string scheme = builder.Scheme;
                        scheme = scheme.Replace("https", "http");
                        builder.Scheme = scheme;
                        builder.Port = 80;
                        request.RequestUri = builder.Uri;
                    }
                    _result = await base.SendAsync(request, cancellationToken);
                    break;
            }

            if (e != null)
            {
                while (e.MoveNext())
                    if (e.Current != null)
                        Debug.Log("e.current " + e.Current as string);
            }

            Debug.Log("Result: " + request.Method);
            Debug.Log("Result: " + _result.ToString());
            Debug.Log("Result Content: " + await _result.Content.ReadAsStringAsync());

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
                Debug.Log("Received from " + method + ": " + uwr.downloadHandler.text);

                _result.StatusCode = (HttpStatusCode)uwr.responseCode;
                _result.ReasonPhrase = _result.StatusCode.ToString();
                _result.Content = new StringContent(uwr.downloadHandler.text, Encoding.UTF8, "application/json");
                yield return uwr.downloadHandler.text;
            }
        }
    }
}
