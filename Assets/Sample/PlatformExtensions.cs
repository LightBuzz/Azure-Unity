using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Sample
{
    public enum Os
    {
        Editor,
        Android,
        Windows,
        UWP,
        iOS
    }
    public static class PlatformExtensions
    {
        public static Os OperatingSystem
        {
            get
            {
#if UNITY_ANDROID
                return Os.Android;
#endif
#if UNITY_STANDALONE_WIN
                return Os.Windows;
#endif
#if UNITY_WSA
                return Os.UWP;
#endif
#if UNITY_IOS
                return Os.iOS;
#endif
                return Os.Editor;
            }
        }
    }
}
