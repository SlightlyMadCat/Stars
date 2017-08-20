using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyVehicleSpeed : MonoBehaviour
{
    public GameObject buyPanel;     //панель для покупки новой машины
    public GameObject readyVehiclePanel;        //панель уже купленной машины
    public GameObject emptyScreenPanel;

    public Text nameVehicle;        //название машины
    public Image icon;         //иконка машины
    public Text vehicleCapacity;        //вместительность
    public Button updBtn;               //кнопка апд

    public bool isBuyPanel;         //если это панель лдя покупки, а не панель в меню

    BusStation busStation;
    BusScreenCont parent;
    Economics economics;

    public int carPrefIndex;
    int slotParentIdex;     //номер слота, откуда вызвали панель
    //int startCarIndex;      //машина, которая уже куплена  этом слоте
    int currentVehicleIndex;
    double price = 0;

    private void Start()
    {
        economics = Economics.SharedInstance;
    }

    public void GetDataFromCont(int _index, int _num)           //если это панель в меню
    { 
        if (busStation == null)
            busStation = GetComponentInParent<BusScreenCont>().busStation;

        carPrefIndex = _index;
        slotParentIdex = _num;

        if (_index > -1)
        {
            readyVehiclePanel.SetActive(true);          //если куплен, то панель апд

            Car car = busStation.carPrefabs[_index].GetComponent<Car>();
            nameVehicle.text = car.name;
            icon.sprite = car.icon;
            vehicleCapacity.text = car.capacity+"";
        } else
        {     //только одну кнопку включить
            if(GetComponentInParent<BusScreenCont>().carIndexes[_num-1] != -1)
            {
                emptyScreenPanel.SetActive(false);
                buyPanel.SetActive(true);
            } else
            {
                emptyScreenPanel.SetActive(true);   //если пустой слоьт
            }
        }
    }

    public void GetBuyData(int _slotIndex, BusScreenCont _parent, int _thisCarIndex)            //если это панель в магазине покупок автобусов
    {
        slotParentIdex = _slotIndex;
        currentVehicleIndex = _thisCarIndex;

        Car car =_parent.busStation.carPrefabs[_thisCarIndex].GetComponent<Car>();
        nameVehicle.text = car.name;
        icon.sprite = car.icon;
        vehicleCapacity.text = car.capacity+"";

        price = car.price;

        parent = _parent;

        int counter = 0;
        for (int i = 0; i < parent.carIndexes.Count; i++)           //сколько машин такого типа есть
        {
            if (parent.carIndexes[i] == _thisCarIndex)
            {
                counter++;
                //print("contain");
            } 
        }

        if(counter >= 1)            //тут прописать форумул для изменения цены
        {
            price += price * counter;
            //print(price);
            //print(counter);
        }
        //print(counter);
        if (economics == null)
            economics = Economics.SharedInstance;

        updBtn.GetComponentInChildren<Text>().text = economics.getShortIndex(price);
    }

    public void CheckBtn()         //проверка хватает ли денег
    {
        if (isBuyPanel)
        {
            if (economics == null)
                economics = Economics.SharedInstance;

            if (economics.totalMoney >= price)
                updBtn.interactable = true;
            else
                updBtn.interactable = false;
        }
    }

    private void FixedUpdate()
    {
        CheckBtn();
    }

    public void SendDataToCont()    //посылаю данные в конт
    {
        parent.SetNewCar(slotParentIdex, currentVehicleIndex);
        economics.ImmidUpd(-price);
        economics.GoldenEggUpd(1);
    }

    public void ShowBuyBusPanel()       //открыть панель покупок
    {
        UIManager.SharedInstance.ShowBuyBuScreen(1);

        int _i = 0;
        foreach (GameObject _panel in GetComponentInParent<BusScreenCont>().buyPanels)
        {
            if (carPrefIndex >= _i)     //если эта машина уже куплена
            {
                _panel.SetActive(false);
            } else
            {
                _panel.SetActive(true);
                GetComponentInParent<BusScreenCont>().buyPanels[_i].GetComponent<BuyVehicleSpeed>().GetBuyData(slotParentIdex, GetComponentInParent<BusScreenCont>(), _i);      //натсроить эту панель для покупки
            }
            _i++;
        }

        UIManager.SharedInstance.uiAnimators[12].gameObject.GetComponentInChildren<Scrollbar>().value = 1;
        GetComponentInParent<BusScreenCont>().UpdAvailableBtn();
    }
}
