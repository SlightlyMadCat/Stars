using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnhanceExample : MonoBehaviour
{
    // Simple Enhance implementation for our UI to use:
    public GameObject canvas;
    public BoostManager boostManaer;
    public UIManager uiManager;

    public void ShowInterstitialAd()
    {
        FGLEnhance.ShowInterstitialAd(FGLEnhance.INTERSTITIAL_PLACEMENT_DEFAULT);
       // Log("Showing interstitial ad");
    }

    public void ShowRewardedAd()
    {
        canvas.SetActive(false);
        FGLEnhance.ShowRewardedAd(FGLEnhance.REWARDED_PLACEMENT_NEUTRAL, OnRewardGranted, OnRewardDeclined, OnRewardUnavailable);
       // Log("Showing rewarded ad");
    }

    public void ShowHelperAd()
    {
        FGLEnhance.ShowRewardedAd(FGLEnhance.REWARDED_PLACEMENT_HELPER, OnRewardGranted, OnRewardDeclined, OnRewardUnavailable);
       // Log("Showing rewarded ad");
    }

    public void ShowSuccessAd()
    {
        FGLEnhance.ShowRewardedAd(FGLEnhance.REWARDED_PLACEMENT_SUCCESS, OnRewardGranted, OnRewardDeclined, OnRewardUnavailable);
        //Log("Showing rewarded ad");
    }

    public void LogEvent()
    {
        FGLEnhance.LogEvent("test_event");
       // Log("Sent analytics event 'test_event'");
    }



    // Callbacks for our rewarded ad:

    private void OnRewardGranted(FGLEnhance.RewardType rewardType, int count)
    {
        if (rewardType == FGLEnhance.RewardType.REWARDTYPE_COINS)
        {
            if (uiManager.uiAnimators[19].gameObject.GetComponent<Animator>().GetInteger("State") != 1)     //если это не реклама по уведомлению
            {
                //print("hide");
                boostManaer.DoubleMoney();
            } else
            {
                Economics.SharedInstance.WatchVideoAndHide();
            }

            canvas.SetActive(true);
            // Log(string.Format("Granted reward of {0} coins", count));
        }
        else
        {
            if (uiManager.uiAnimators[19].gameObject.GetComponent<Animator>().GetInteger("State") != 1)     //если это не реклама по уведомлению
            {
                //print("hide");
                boostManaer.DoubleMoney();
            }
            else
            {
                Economics.SharedInstance.WatchVideoAndHide();
            }

            canvas.SetActive(true);
            //Log(string.Format("Granted reward"));
        }
    }

    private void OnRewardDeclined()
    {
        //Log(string.Format("Reward declined"));
        canvas.SetActive(true);
    }

    private void OnRewardUnavailable()
    {
        //Log(string.Format("Reward unavailable"));
        canvas.SetActive(true);
    }

    private void Update()
    {
        if (!canvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                canvas.SetActive(true);
            }
        }
    }
}
