using System.Net;
using Microsoft.WindowsAzure.MobileServices;
using UnityEngine;

namespace Assets.Sample
{
    public class Sample : MonoBehaviour
    {
        [SerializeField]
        protected string MobileAppUri = "https://testtodolightbuzz.azurewebsites.net";
        protected MobileServiceClient AzureClient;
        // Use this for initialization
        void Start()
        {
            var handler = new LightBuzzHttpsHandler() { AutomaticDecompression = DecompressionMethods.Deflate };
            AzureClient = new MobileServiceClient(MobileAppUri, handler);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
