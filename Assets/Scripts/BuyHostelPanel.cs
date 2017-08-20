using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyHostelPanel : MonoBehaviour {
    public Image icon;      //hostels icon
    public Text nameText;         //hostels name
    public Button buyBtn;  //hostel buy
    Text price;
    public Text capacity;        //hostel capacity
    public UpgradesPanel _updPanel;     //upd panel
    public Economics economics;
    double currentHostelPrice;
    public int hostelToBuildIndex;      //индекс префаба хостела, который эта панель покупает
    int buildedSimilarHostels = 0;

    private void Start()
    {
        price = buyBtn.GetComponentInChildren<Text>();
    }

    public void UpdHostelPrice()        //апдейт цены, если построено несколько одинаковых хостелов
    {
        //print("3");
        //TODO
        buildedSimilarHostels = 1;
        foreach(StarHostel hostel in CloneCenter.SharedInstance.starHostels)
        {
            if (hostel.currentHostelIndex == hostelToBuildIndex)
            {
                buildedSimilarHostels++;
            }
        }

        if (buildedSimilarHostels > 0)
        {
            currentHostelPrice = currentHostelPrice * buildedSimilarHostels;       //сделать не линейную формулу
        }
    }

    public void SetDataEndUpdate()          //покупка
    {
        _updPanel.SendUpdate(hostelToBuildIndex);
        //UpdHostelPrice();
        //economics.totalMoney -= currentHostelPrice;
        economics.ImmidUpd(-currentHostelPrice);
        economics.GoldenEggUpd(1);
    }

    public void SetHostelTTX(int _index)            //ТТХ ХОСТЕЛА НА ПАНЕЛИ
    {
        //print("1");
        if (UIManager.SharedInstance.uiAnimators[1].GetInteger("State") != 1)
            return;

        hostelToBuildIndex = _index;
        CurrentHostel _hostel = GameObject.FindGameObjectWithTag("Hostel").GetComponent<StarHostel>().hostelBuildings[_index].GetComponent<CurrentHostel>();        //ССЫЛКА НА ТТХ

        currentHostelPrice = _hostel.price;
        UpdHostelPrice();

        icon.GetComponent<Image>().sprite = _hostel.icon;
        nameText.text = _hostel.name;

        if (economics == null)
            economics = Economics.SharedInstance;

        price.text = economics.getShortIndex(currentHostelPrice) + "$";
        capacity.text = _hostel.thisHostelCapacity.ToString();

        CheckIfInterectable(_hostel);
        //TODO: STRING FOR BIG NUMBERS
    }

    public void CheckIfInterectable(CurrentHostel _hostel)          //проверка только на достаточность денег
    {
        //print("2");
        if (currentHostelPrice > economics.totalMoney)
        {   
            if(buyBtn.interactable == true)
                buyBtn.interactable = false;
        }
        else
        {
            if(buyBtn.interactable == false)
                buyBtn.interactable = true;
        }
    }
}
