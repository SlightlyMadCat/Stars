using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public static bool played = false;

    void Awake()
    {
        if (played == false) {
            DontDestroyOnLoad(transform.gameObject);
            played = true;
        }
    }
}
