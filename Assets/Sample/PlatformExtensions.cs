using System;
using System.IO;
using UnityEngine;

namespace LightBuzz
{
    public enum TargetPlatform
    {
        UnityEditor,
        Android,
        iOS,
        Windows,
        UWP
    }

    public static class PlatformTools
    {
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
#else
                return Os.UnityEditor;
#endif
            }
        }

        public static void CopyDatabase(string localDatabaseName, string localDatabasePath)
        {
            switch (TargetPlatform)
            {
                case TargetPlatform.Android:
                    {
                        WWW original = new WWW("jar:file://" + Application.dataPath + "!/assets/" + localDatabaseName);
                        while (!original.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check.
                        File.WriteAllBytes(localDatabasePath, original.bytes);
                    }
                    break;
                case TargetPlatform.iOS:
                    {
                        string original = Path.Combine(Application.dataPath, "Raw", localDatabaseName);
                        File.Copy(original, localDatabasePath);
                    }
                    break;
                case TargetPlatform.UWP:
                case TargetPlatform.Windows:
                case TargetPlatform.UnityEditor:
                    {
                        string original = Path.Combine(Application.dataPath, "StreamingAssets", localDatabaseName);
                        File.Copy(original, localDatabasePath);
                    }
                    break;
                default:
                    throw new Exception("You need to provide an empty database file in your StreamingAssets folder.");
            }
        }
    }
}
