using UnityEngine;

namespace FGL.Enhance.Internals
{
    internal class FGLAndroidInternals
    {
        public static void Initialize()
        {
        }

        public static bool IsInterstitialReady(string placement)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            return adConnectorClass.CallStatic<bool>("isInterstitialReady", placement);
        }

        public static void ShowInterstitialAd(string placement)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            adConnectorClass.CallStatic("showInterstitialAd", placement);
        }

        public static bool IsRewardedAdReady(string placement)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            return adConnectorClass.CallStatic<bool>("isRewardedAdReady", placement);
        }

        public static void ShowRewardedAd(string placement, string bridgeObjectName)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            adConnectorClass.CallStatic("showRewardedAd", placement, bridgeObjectName);
        }

        public static void LogEvent(string eventType)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            adConnectorClass.CallStatic("logEvent", eventType, null, null);
        }

        public static void LogEvent(string eventType, string paramKey, string paramValue)
        {
            AndroidJavaClass adConnectorClass = new AndroidJavaClass("com.fgl.enhance.unityconnector.FGLUnityConnector");
            adConnectorClass.CallStatic("logEvent", eventType, paramKey, paramValue);
        }

    }
}