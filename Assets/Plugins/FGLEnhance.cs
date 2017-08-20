/**
 * Enhance connector for Unity
 */

using UnityEngine;
using System;
using FGL.Enhance.Internals;

public class FGLEnhance
{
    ///// PUBLIC /////

    // Where the rewarded as was triggered
    public const string REWARDED_PLACEMENT_SUCCESS = "SUCCESS";                 // Ad following success in the game, such as completing a level
    public const string REWARDED_PLACEMENT_HELPER = "HELPER";                   // Ad to help the user, for instance after losing a level or when needing currency
    public const string REWARDED_PLACEMENT_NEUTRAL = "NEUTRAL";                 // Ad in a neutral circumstance, for instance the user tapping "Watch video for a reward"

    // Where the interstitial was triggered
    public const string INTERSTITIAL_PLACEMENT_DEFAULT = "default";             // Default ad placement
    public const string INTERSTITIAL_LEVEL_COMPLETED = "level_completed";       // Level completed

    // Type of reward that was granted
    public enum RewardType
    {
        REWARDTYPE_ITEM,           // Arbitrary game-defined item granted (no coin amount reported)
        REWARDTYPE_COINS          // A defined coins amount was granted
    };

    /**
     * Check if an interstitial ad is ready
     * 
     * @return true if an interstitial ad is ready, false if not
     */
    public static bool IsInterstitialReady(string placement = INTERSTITIAL_PLACEMENT_DEFAULT)
    {
        InitializeEnhance();

#if UNITY_EDITOR
        return FGLEditorInternals.IsInterstitialReady(placement);
#elif UNITY_ANDROID
        return FGLAndroidInternals.IsInterstitialReady(placement);
#elif UNITY_IOS
        return FGLiOSInternals.IsInterstitialReady(placement);
#else
        return true;
#endif
    }

    /**
     * Show interstitial ad
     */
    public static void ShowInterstitialAd(string placement = INTERSTITIAL_PLACEMENT_DEFAULT)
    {
        InitializeEnhance();

#if UNITY_EDITOR
        FGLEditorInternals.ShowInterstitialAd(placement);
#elif UNITY_ANDROID
        FGLAndroidInternals.ShowInterstitialAd(placement);
#elif UNITY_IOS
        FGLiOSInternals.ShowInterstitialAd(placement);
#endif
    }

    /**
     * Check if a rewarded ad is ready
     * 
     * @return true if a rewarded ad is ready, false if not
     */
    public static bool IsRewardedAdReady(string placement = REWARDED_PLACEMENT_NEUTRAL)
    {
        InitializeEnhance();

#if UNITY_EDITOR
        return FGLEditorInternals.IsRewardedAdReady(placement);
#elif UNITY_ANDROID
        return FGLAndroidInternals.IsRewardedAdReady(placement);
#elif UNITY_IOS
        return FGLiOSInternals.IsRewardedAdReady(placement);
#else
        return false;
#endif
    }

    /**
      * Show rewarded ad
      * 
      * @param placement placement type for this ad
      * @param onRewardGrantedCallback callback executed when the ad reward is granted
      * @param onRewardDeclinedCallback callback executed when the ad reward is declined
      * @param onRewardUnavailableCallback callback executed when the ad reward is unavailable
      */
    public static void ShowRewardedAd(string placement, Action<RewardType, int> onRewardGrantedCallback, Action onRewardDeclinedCallback, Action onRewardUnavailableCallback)
    {
        if (GameObject.Find(FGLEnhance_Callbacks.CallbackObjectName) == null)
        {
            string newName = "__FGLEnhance_Callback_" + UnityEngine.Random.Range(0, int.MaxValue);
            GameObject callbackObject = new GameObject(newName);
            callbackObject.AddComponent<FGLEnhance_Callbacks>();
        }

        FGLEnhance_Callbacks.OnRewardGrantedCallback = onRewardGrantedCallback;
        FGLEnhance_Callbacks.OnRewardDeclinedCallback = onRewardDeclinedCallback;
        FGLEnhance_Callbacks.OnRewardUnavailableCallback = onRewardUnavailableCallback;

        InitializeEnhance();
#if UNITY_EDITOR
        FGLEditorInternals.ShowRewardedAd(placement, FGLEnhance_Callbacks.CallbackObjectName);
#elif UNITY_ANDROID
        FGLAndroidInternals.ShowRewardedAd(placement, FGLEnhance_Callbacks.CallbackObjectName);
#elif UNITY_IOS
        FGLiOSInternals.ShowRewardedAd(placement, FGLEnhance_Callbacks.CallbackObjectName);
#endif
    }

    /**
     * Log custom analytics event
     * 
     * @param eventType event type (for instance 'menu_shown')
     */
    public static void LogEvent(string eventType)
    {
        InitializeEnhance();

#if UNITY_EDITOR
        FGLEditorInternals.LogEvent(eventType);
#elif UNITY_ANDROID
        FGLAndroidInternals.LogEvent(eventType);
#elif UNITY_IOS
        FGLiOSInternals.LogEvent(eventType);
#endif
    }

    /**
     * Log custom analytics event
     * 
     * @param eventType event type (for instance 'level_completed')
     * @param paramKey parameter key (for instance 'level')
     * @param paramValue parameter value (for instance '3')
     */
    public static void LogEvent(string eventType, string paramKey, string paramValue)
    {
        InitializeEnhance();

#if UNITY_EDITOR
        FGLEditorInternals.LogEvent(eventType, paramKey, paramValue);
#elif UNITY_ANDROID
        FGLAndroidInternals.LogEvent(eventType, paramKey, paramValue);
#elif UNITY_IOS
        FGLiOSInternals.LogEvent(eventType, paramKey, paramValue);
#endif
    }

    ///// PRIVATE /////
    static void InitializeEnhance()
    {
        if (!mInitialized)
        {
            mInitialized = true;

#if UNITY_EDITOR
            FGLEditorInternals.Initialize();
#elif UNITY_ANDROID
            FGLAndroidInternals.Initialize();
#elif UNITY_IOS
            FGLiOSInternals.Initialize();
#endif
        }
    }

    static bool mInitialized = false;
}
