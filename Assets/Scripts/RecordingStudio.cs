using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class RecordingStudio : MonoBehaviour {

    public static RecordingStudio SharedInstance;
    public Economics economics;
    public UIManager uiManager;

    double earnedMoney;
    public GameObject[] buildings;      //префабы зданий для разных уровней
    public LevelManager levelManager;

    public int silosCount = 1;      //колво слотов для афк
    double newNumClones = 0;

    [Header("PANEL UI")]
    public Image[] icons;
    public Sprite defaultSprite;
    public Sprite boughtSprite;
    public Text afkHoursText;
    public Button buyBtn;
    public double siloPrice;
    public GameObject panel;
    public bool visible = false;

    private void Awake()
    {
        SharedInstance = this;
    }

    public void SetBuildingPrefab()     //выбираю какой префаб здания показать
    {
        int i = levelManager.currentLevel;
        int index = 0;

        if (i >= 0 && i < 3)
        {
            index = 0;
        } else if (i >= 3 && i < 6)
        {
            index = 1;
        }
        else if (i >= 6 && i < 9)
        {
            index = 2;
        }
        else if (i >= 9 && i < 12)
        {
            index = 3;
        }
        else if (i >= 12 && i < 15)
        {
            index = 4;
        }

        buildings[index].SetActive(true);
    }

    public void getSaveData(DateTime _exitTime)
    {
        SetBuildingPrefab();

        DateTime currentTime = DateTime.Now;
        DateTime exitTime = _exitTime;
        TimeSpan difference;                //длительность времени афк

        difference = currentTime - exitTime;
        if (difference.TotalMinutes < 1)            //если прошло меньше минуты
            return;

        earnedMoney = /*economics.layingRate * */difference.TotalMilliseconds * economics.CalculateFV() * economics.layingRate * CloneCenter.SharedInstance.clonesNum;

        //РОСТ КОЛ-ВА КЛОНОВ

        double maxAFKminutes = 0;
        if (difference.TotalMinutes > silosCount * 60) //если афк было дольше чем лимит, берем верхний потолок
        {    
            maxAFKminutes = silosCount * 60;
        }
        else
        {
            maxAFKminutes = difference.TotalMinutes;
        }

        newNumClones = CloneCenter.SharedInstance.clonesNum * (Math.Pow((1 + (CloneCenter.SharedInstance.cloneLayingRate * 6000d)), maxAFKminutes));
        //print(newNumClones);
        if (CloneCenter.SharedInstance.availableSlotsHostel > (int)newNumClones)  //если мест хватает
        {
            newNumClones = (int)newNumClones - CloneCenter.SharedInstance.clonesNum;
            //print(newNumClones +" have places");
            //print(difference.TotalMinutes);
        } else
        {
            newNumClones = CloneCenter.SharedInstance.availableSlotsHostel - CloneCenter.SharedInstance.clonesNum;
            //print(newNumClones + " not enaugh places");
        }

        ShowUI();
    }

    void ShowUI()           //показать ui уведомление
    {
        if (earnedMoney <= 0)
            return;

        uiManager.ShowAfkScreen(1);
        uiManager.uiAnimators[13].gameObject.GetComponentInChildren<Text>().text = economics.getShortIndex(earnedMoney) + " $";

        if (newNumClones > 0)
        {
            uiManager.uiAnimators[13].gameObject.GetComponentsInChildren<Text>()[1].text = "New stars : " + economics.getShortIndex(newNumClones);
        } else
        {
            uiManager.uiAnimators[13].gameObject.GetComponentsInChildren<Text>()[1].text = "New stars : 0";
        }
    }

    public void HideFarmNotif()          //скрыть скрин уведомлений и начислить деньги
    {
        uiManager.ShowAfkScreen(2);
        economics.ImmidUpd(earnedMoney);
        CloneCenter.SharedInstance.SendNewStarsToHostel((int)newNumClones);
    }

    public void WarpHours(int _i)           //пропустить n часов
    {
        double allMsc = _i * 3600000;

        if (economics.moneyPerSecond <= 0)
            return;

        earnedMoney = allMsc * economics.moneyPerSecond * economics.layingRate * economics.numOfClones;
        //print(earnedMoney);

        newNumClones = CloneCenter.SharedInstance.clonesNum * (Math.Pow((1 + (CloneCenter.SharedInstance.cloneLayingRate * 6000d)), _i*60));
        //print(newNumClones);
        if (CloneCenter.SharedInstance.availableSlotsHostel > (int)newNumClones)  //если мест хватает
        {
            newNumClones = (int)newNumClones - CloneCenter.SharedInstance.clonesNum;
            //print(newNumClones +" have places");
            //print(difference.TotalMinutes);
        }
        else
        {
            newNumClones = CloneCenter.SharedInstance.availableSlotsHostel - CloneCenter.SharedInstance.clonesNum;
            //print(newNumClones + " not enaugh places");
        }

        ShowUI();
    }

    public void UpdSilosCount(int _new)
    {
        if (silosCount >= 10)
        {
            buyBtn.interactable = false;
            afkHoursText.text = silosCount + "";
        }
        else
        {
            silosCount += _new;
            afkHoursText.text = silosCount + "";

            if (silosCount >= 10)
                buyBtn.interactable = false;
        }

        int i = 0;
        foreach (Image _image in icons)
        {
            if (i < silosCount)
            {
                _image.sprite = boughtSprite;
            }
            else
            {
                _image.sprite = defaultSprite;
            }
            i++;
        }

        if(_new > 0)
        {
            economics.ImmidUpd(-siloPrice);
            CheckBuyBtn();
        }

        CheckBuyBtn();
    }

    public void CheckBuyBtn()
    {
        //if (!visible)
        //    return;

        if(silosCount >= 1 && silosCount < 10)
        {
            buyBtn.GetComponentInChildren<Text>().text = "Upgrade studio : "+siloPrice + " $";
            if (economics.totalMoney > siloPrice)
                buyBtn.interactable = true;
            else
                buyBtn.interactable = false;
        }
    }

    private void FixedUpdate()
    {
        if (panel.transform.position.x < 600)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            CheckBuyBtn();
        }
        else if (transform.position.x > 600 && visible == true)
        {
            visible = false;
        }
    }
}
