using UnityEngine;
using System.Runtime.InteropServices;

namespace FGL.Enhance.Internals
{
    internal class FGLiOSInternals
    {
        [DllImport ("__Internal")]
        private static extern bool FGLConnector_isInterstitialAdReady(string placement);

        [DllImport ("__Internal")]
        private static extern void FGLConnector_showInterstitialAd(string placement);

        [DllImport ("__Internal")]
        private static extern bool FGLConnector_isRewardedAdReady(string placement);

        [DllImport ("__Internal")]
        private static extern void FGLConnector_showRewardedAd(string placement, string bridgeObjectName);

        [DllImport ("__Internal")]
        private static extern void FGLConnector_logEvent(string eventType, string paramKey, string paramValue);

        public static void Initialize()
        {
        }

        public static bool IsInterstitialReady(string placement)
        {
            return FGLConnector_isInterstitialAdReady(placement);
        }

        public static void ShowInterstitialAd(string placement)
        {
            FGLConnector_showInterstitialAd(placement);
        }

        public static bool IsRewardedAdReady(string placement)
        {
            return FGLConnector_isRewardedAdReady(placement);
        }

        public static void ShowRewardedAd(string placement, string bridgeObjectName)
        {
            FGLConnector_showRewardedAd(placement, bridgeObjectName);
        }

        public static void LogEvent(string eventType)
        {
            FGLConnector_logEvent(eventType, null, null);
        }

        public static void LogEvent(string eventType, string paramKey, string paramValue)
        {
            FGLConnector_logEvent(eventType, paramKey, paramValue);
        }
    }
}