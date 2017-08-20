using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusScreenCont : MonoBehaviour {
    public GameObject[] busPanels;          
    public int currentNumPlaces;        //сколько сейас доступно слотов
    public int buySlots;                //сколько слотов куплено
    public List<int> carIndexes = new List<int>();      //список купленых префабов   --если менять кол-во то вместе со скриптом сохранов
    public BusStation busStation;
    public GameObject[] buyPanels;          //список панелей в окне покупки автобуса
    public double totalCapacity = 0;
    public double currentCapacity = 0;
    public Image capacityScale;
    public Text capacityText;

    public GameObject capacityNotif;

    public CarSpawner carSpawner;
    public Economics economics;
    public UIManager uiManager;
    public CloneCenter cloneCenter;

    public GameObject warning;

    public bool visible = false;
    public List<float> capacityList = new List<float>();

    private void Start()
    {
        //SetSlots();
        //UpdScale();
        //print(int.MaxValue);
    }

    private void FixedUpdate()
    {
        if (transform.position.x < 600 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            busStation.scrollBar.value = 1;
            UpdScale();
        }
        else if (transform.position.x > 600 && visible == true)
        {
            visible = false;
        }
    }

    public void SetSlots()          //set slots
    {
        double curCapacity = 0;
        for (int i = 0; i < busPanels.Length; i++)
        {
            if(i < currentNumPlaces)
            {
                //print(carIndexes[i]+" index");
                busPanels[i].SetActive(true);
                busPanels[i].GetComponent<BuyVehicleSpeed>().GetDataFromCont(carIndexes[i],i);        //отсылаю номер префаба на панель

                if (carIndexes[i] >= 0)
                {
                    curCapacity += busStation.carPrefabs[carIndexes[i]].GetComponent<Car>().capacity;
                }
            } else
            {
                busPanels[i].SetActive(false);
            }
        }

        UpdCapacity(curCapacity);
        carSpawner.SetCarsToSpawnList();
        busStation.scrollBar.value = 1;
    }
    
    public void UpdCapacity(double _capacity)       //обновить вместимость
    {
        totalCapacity = _capacity;
        capacityText.text = totalCapacity + "";
        //UpdScale();
    }

    public void SetNewCar(int _slotIndex, int _vehIndex)        //апд машины
    {
        carIndexes[_slotIndex] = _vehIndex;
        //carIndexes[1] = 0;
        SetSlots();
        busPanels[_slotIndex].GetComponent<BuyVehicleSpeed>().ShowBuyBusPanel();
        busStation.scrollBar.value = 1;
    }

    public void UpdAvailableBtn()
    {
        if (!visible)
            return;

        //print(GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().uiAnimators[12].GetInteger("State"));
        //if (GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().uiAnimators[12].GetInteger("State") != 1)
        //    return;

        foreach (GameObject _panel in buyPanels)
        {
            _panel.GetComponent<BuyVehicleSpeed>().CheckBtn();
        }
    }

    public void UpdScale()
    {
        if (economics.moneyPerSecond <= 0)
            return;

            float f = (float)((60000d * economics.layingRate * cloneCenter.clonesNum) / totalCapacity);

        if (f < 1)
        {
            if (capacityNotif.activeSelf)
            {
                Setup(false);
            }
        }
        else
        {
            if (!capacityNotif.activeSelf)
            {
                Setup(true);
            }
        }

        if (!visible)
            return;

            //float f = (float)((cloneCenter.clonesNum * economics.eggPerSecond)/totalCapacity);
            capacityScale.fillAmount = f;
            capacityScale.color = Color.Lerp(Color.green, Color.red, f);
            //print(f);
            //print(economics.eggPerSecond + " eps");
        //}
    }

    void Setup(bool _val)
    {
        //print(_val);
        capacityNotif.SetActive(_val);
        cloneCenter.enaughVeh = !_val;
        warning.SetActive(_val);
    }
}
