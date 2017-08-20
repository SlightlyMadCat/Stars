using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static string levelName;
    
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
