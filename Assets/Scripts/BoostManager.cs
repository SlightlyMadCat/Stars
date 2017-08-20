using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BoostManager : MonoBehaviour {

    public static BoostManager SharedInstance;

    System.DateTime startTime;
    bool startTimer = false;
    public Text timeText;       //текст, куда выведено время infinity

    public DateTime startDoubleBoost;
    public GameObject boostText;
    public bool boostTimer = false;

    public RecordingStudio recStudio;
    public Economics economics;
    public CloneCenter cloneCenter;

    public Button infSpawn;
    public Button warpOneHour;
    public Button warpEightHours;
    
    public bool visible = false;
    public Transform panel;

    private void Awake()
    {
        SharedInstance = this;
    }

    public void BoostChikens()      //вкл бесконечный спаун куриц
    {
            GameObject.FindGameObjectWithTag("CloneCenter").GetComponent<CloneCenter>().isInfinity = true;
            startTimer = true;
            startTime = System.DateTime.Now;

            economics.GoldenEggUpd(-2);
    }

    public void WarpTime(int _goldenCoins)       //пропустить 1 hour
    {
        recStudio.WarpHours(1);
        economics.GoldenEggUpd(-_goldenCoins);
    }

    public void WarpEightHours(int _goldenCoins)                //warp 8 hours
    {
         recStudio.WarpHours(8);
         economics.GoldenEggUpd(-_goldenCoins);
    }

    public void DoubleMoney()               //вкл буст на 2 часа
    {
        //startDoubleBoost = System.DateTime.Now;
        SetupBoost(DateTime.Now);
    }

    public void SetupBoost(DateTime _startTime)
    {
        startDoubleBoost = _startTime;
        economics.boostK = 2;
        boostTimer = true;
        boostText.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (startTimer)         //таймер на 30 сек
        {
            TimeSpan dif = DateTime.Now - startTime;
            timeText.text = 30 - dif.Seconds + "";

            if (dif.Seconds >= 30)
            {
                cloneCenter.isInfinity = false;
                startTimer = false;
                timeText.text = "";
            }
        }

        if (boostTimer)             //таймер на 2 часа
        {
            TimeSpan dif = DateTime.Now - startDoubleBoost;
            //print(dif.Hours);

            if(dif.TotalHours >= 2)
            {
                economics.boostK = 1;
                boostText.SetActive(false);
                boostTimer = false;
            }
        }

        if (panel.transform.position.x < 600 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            CheckInteractBtn();
        }
        else if (transform.position.x > 600 && visible == true)
        {
            visible = false;
        }
    }

    public void CheckInteractBtn()         //проверка на доступность всех внопок
    {
        if (economics.goldCoins >= 2)
            infSpawn.interactable = true;
        else
            infSpawn.interactable = false;

        if (economics.goldCoins >= 25)
            warpOneHour.interactable = true;
        else
            warpOneHour.interactable = false;

        if (economics.goldCoins >= 100)
            warpEightHours.interactable = true;
        else
            warpEightHours.interactable = false;
    }
}
