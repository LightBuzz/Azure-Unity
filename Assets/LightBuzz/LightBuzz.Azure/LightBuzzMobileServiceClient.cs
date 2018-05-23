using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using UnityEngine;

namespace LightBuzz.Azure
{
    public abstract class LightBuzzMobileServiceClient : MobileServiceClient
    {
        protected abstract void DefineTables();

        /// <summary>
        /// Pulls the data from the remote Azure App Service and stores them into the local database.
        /// </summary>
        /// <returns></returns>
        public abstract Task Pull();

        /// <summary>
        /// Specifies whether the app will store data locally.
        /// </summary>
        public bool SupportsLocalStore { get; set; }

        protected readonly string DefaultLocalDatabaseNamePattern = "database_{0}.db";
        public string DatabaseName { get; set; }

        private string _localDatabasePath;

        protected MobileServiceSQLiteStore LocalStore;


#if !UNITY_WSA || UNITY_EDITOR

        protected LightBuzzMobileServiceClient(string mobileAppUri, bool supportLocal) : base(mobileAppUri, new LightBuzzHttpsHandler())
        {
            SupportsLocalStore = supportLocal;
            DatabaseName = string.Format(DefaultLocalDatabaseNamePattern, mobileAppUri.GetHashCode());
        }
#else

        protected LightBuzzMobileServiceClient(string mobileAppUri,bool supportLocal) : base(mobileAppUri)
        {
            SupportsLocalStore = supportLocal;
            DatabaseName = string.Format(DefaultLocalDatabaseNamePattern, mobileAppUri.GetHashCode());
        }

#endif

        public async Task InitializeLocalStore()
        {
            await InitStore();
            await SyncStore();
        }

        public async Task InitStore()
        {
            if (SupportsLocalStore)
            {
                await Init(this);
            }
        }

        public async Task SyncStore()
        {
            if (SupportsLocalStore)
            {
                await Sync();
            }
        }

        protected string LocalDatabasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_localDatabasePath))
                {
                    return Path.Combine(Application.persistentDataPath, DatabaseName);
                }

                return _localDatabasePath;
            }
        }

        protected string LocalDatabaseConnectionString
        {
            get
            {
#if !UNITY_WSA || UNITY_EDITOR
                return "Data Source=" + LocalDatabasePath + ";Version=3;";
#else
                return LocalDatabasePath;
#endif
            }
        }

        /// <summary>
        /// Initializes the local SQLite database.
        /// </summary>
        /// <param name="azureClient">The Azure Client object.</param>
        /// <returns></returns>
        public async Task Init(LightBuzzMobileServiceClient azureClient)
        {
            await Init(azureClient, Path.Combine(Application.persistentDataPath, DatabaseName));
        }

        /// <summary>
        /// Initializes the local SQLite database.
        /// </summary>
        /// <param name="azureClient">The Azure Client object.</param>
        /// <param name="localDatabasePath">The full path to the local database file, e.g. Path.Combine(Application.persistentDataPath, "database.db").</param>
        /// <returns></returns>
        public async Task Init(LightBuzzMobileServiceClient azureClient, string localDatabasePath)
        {
            if (azureClient == null)
            {
                throw new NullReferenceException("Azure Client is null.");
            }

            _localDatabasePath = localDatabasePath;

            if (!azureClient.SyncContext.IsInitialized)
            {
                try
                {
#if !UNITY_WSA
                    if (!File.Exists(LocalDatabasePath))
                    {
                        File.Create(LocalDatabasePath).Close();
                    }
#endif

                    LocalStore = new MobileServiceSQLiteStore(LocalDatabaseConnectionString);

                    DefineTables();

                    await SyncContext.InitializeAsync(LocalStore);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(e.Source);
                    Debug.LogError(e.StackTrace);
                    Debug.LogError(e.ToString());
                }
            }
        }

        /// <summary>
        /// Syncs with the remote Azure App Service (pull/push operations).
        /// </summary>
        /// <returns></returns>
        public async Task Sync()
        {
            try
            {
                await Push();
                await Pull();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.Source);
                Debug.LogError(e.StackTrace);
                Debug.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Pushes the data stored in the local SQLite database to the remote Azure App Service.
        /// </summary>
        /// <returns></returns>
        public async Task Push()
        {
            await SyncContext.PushAsync();
        }
    }
}
