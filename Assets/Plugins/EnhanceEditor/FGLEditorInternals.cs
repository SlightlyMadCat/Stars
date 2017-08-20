using UnityEngine;

namespace FGL.Enhance.Internals
{
    public class FGLEditorInternals
    {

        public static void Initialize()
        {

        }

        public static bool IsInterstitialReady(string placement)
        {
            return true;
        }

        public static void ShowInterstitialAd(string placement)
        {
            GameObject interstitial = GameObject.Instantiate(Resources.Load<GameObject>("Enhance_PlaceholderInterstitial"));
            interstitial.name = "Example Interstitial Ad";
            Debug.Log("[Enhance] Displaying interstitial ad");
        }

        public static bool IsRewardedAdReady(string placement)
        {
            return true;
        }

        public static void ShowRewardedAd(string placement, string bridgeObjectName)
        {
            GameObject rewarded = GameObject.Instantiate(Resources.Load<GameObject>("Enhance_PlaceholderRewarded"));
            rewarded.name = "Example Rewarded Ad";
            Debug.Log("[Enhance] Displaying rewarded ad");
        }

        public static void GrantReward()
        {
            GameObject callbackClass = GameObject.Find(FGLEnhance_Callbacks.CallbackObjectName);
            if (callbackClass == null) return;
            FGLEnhance_Callbacks enhanceCallbacks = callbackClass.GetComponent<FGLEnhance_Callbacks>();
            enhanceCallbacks.EnhanceOnCoinsRewardGranted("1");
        }

        public static void LogEvent(string eventType)
        {
            Debug.Log(string.Format("[Enhance] Log Event: {0}", eventType));
        }

        public static void LogEvent(string eventType, string paramKey, string paramValue)
        {
            Debug.Log(string.Format("[Enhance] Log Event: {0} ({1} = {2})", eventType, paramKey, paramValue));
        }
    }
}