using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusStation : MonoBehaviour {
    public Scrollbar scrollBar;
    public GameObject[] carPrefabs;
    public GameObject[] stationPrefabs;     //-- меняются в car spawner

    public static BusStation SharedInstance;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        scrollBar.value = 1;        //сброс наверх
    }
}
