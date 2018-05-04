using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LightBuzz
{
    /// <summary>
    /// The target platform of the application.
    /// </summary>
    public enum TargetPlatform
    {
        Android,
        iOS,
        Windows,
        UWP
    }

    /// <summary>
    /// Platform-dependent methods.
    /// </summary>
    public class LocalStore
    {
        private static readonly string LocalDatabaseName = "localstore.db";

        private static MobileServiceSQLiteStore store;

        private static MobileServiceClient _azureClient;

        private static string LocalDatabasePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, LocalDatabaseName);
            }
        }

        public static TargetPlatform TargetPlatform
        {
            get
            {
#if UNITY_ANDROID
                return Os.Android;
#elif UNITY_IOS
                return Os.iOS;
#elif UNITY_STANDALONE_WIN
                return TargetPlatform.Windows;
#elif UNITY_WSA
                return Os.UWP;
#endif
            }
        }

        public static async Task Init(MobileServiceClient azureClient)
        {
            if (azureClient == null)
            {
                throw new NullReferenceException("Azure Client is null.");
            }

            _azureClient = azureClient;

            if (!azureClient.SyncContext.IsInitialized)
            {
                try
                {
                    if (!File.Exists(LocalDatabasePath))
                    {
                        switch (TargetPlatform)
                        {
                            case TargetPlatform.Android:
                                {
                                    WWW original = new WWW("jar:file://" + Application.dataPath + "!/assets/" + LocalDatabaseName);
                                    while (!original.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check.
                                    File.WriteAllBytes(LocalDatabasePath, original.bytes);
                                }
                                break;
                            case TargetPlatform.iOS:
                                {
                                    string original = Path.Combine(Application.dataPath, "Raw", LocalDatabaseName);
                                    File.Copy(original, LocalDatabasePath);
                                }
                                break;
                            case TargetPlatform.UWP:
                            case TargetPlatform.Windows:
                                {
                                    string original = Path.Combine(Application.dataPath, "StreamingAssets", LocalDatabaseName);
                                    File.Copy(original, LocalDatabasePath);
                                }
                                break;
                            default:
                                throw new Exception("You need to provide an empty database file in your StreamingAssets folder.");
                        }
                    }

                    store = new MobileServiceSQLiteStore(LocalDatabasePath);

                    DefineTables();

                    await _azureClient.SyncContext.InitializeAsync(store);
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

        private static void DefineTables()
        {
            // EDIT - Add your own tables here...
            store.DefineTable<TodoItem>();
        }

        public static async Task Sync()
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

        public static async Task Pull()
        {
            // EDIT - Add your own tables here.
            MobileAppsTableDAO<TodoItem> todoTableDAO = await MobileAppsTableDAO<TodoItem>.Init(_azureClient);

            await todoTableDAO.Pull(new CancellationToken(), "todoItems", (x) => x.Id != null);
        }

        public static async Task Push()
        {
            await _azureClient.SyncContext.PushAsync();
        }
    }
}
