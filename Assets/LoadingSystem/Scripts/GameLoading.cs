using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoading : MonoBehaviour
{

    //public KeyCode _keyCode = KeyCode.Space;
    public GameObject loadingInfo, loadingIcon;
    private AsyncOperation async;
    public Image panel;

    IEnumerator Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        async = SceneManager.LoadSceneAsync(GameManager.levelName);
        loadingIcon.SetActive(true);
        //loadingInfo.SetActive(false);
        yield return true;
        async.allowSceneActivation = false;
        //loadingIcon.SetActive(false);
        //loadingInfo.SetActive(true);
    }

    void FixedUpdate()
    {
        //print(async.progress);
        panel.fillAmount = async.progress;
        //if (Input.GetKeyDown(_keyCode)) async.allowSceneActivation = true;
        if (async.progress >= 0.9f)
            async.allowSceneActivation = true;
    }
}
