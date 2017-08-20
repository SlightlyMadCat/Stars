using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager SharedInstance;

   // public float Cd;
    [HideInInspector]
    //public float curCd;
    float fps;

    float deltaTime = 0.0f;         //for fps cpunter
    public Animator setTapField;        //контейнер кнопки спауна
    public Animator fiveButtons;        //контейнер кнопок bottom
    public int setFieldState = 0;       //состояние поля для нажатия спауна
    public Button spawnButton;      //кнопка спауна
    public Image timeSpawnPanel;            //цветная лента спауна
    //public GameObject hostelScreen;         //панель с хостелами
    public Image capacityScale;         //общая вместимость всех хостелов
    public Text totalCapacityText;      //кол-во общих свободных слотов
    public GameObject[] allHostels;         //все хостелы на карте
    //public Animator hostelsUpgrades;        //аниматор ui апгрейдов хостелов
    int hostelUpdState = 0;                 //состояние аниматора
    //public Animator mainMenuAnim;           //аниматор mainmenu
    public Text totalMoneyText;         
    public Text moneyPerSecondText;
    public Text goldenCoinText;
    //public Text tapCount;               //кол-во нажатий за секунду
    public GameObject minTapZone;       //область для нажатий 1
    public GameObject maxTapZone;       //область для нажатий 2
    public Text clonesOnScreenText;         //сколько клонов бегает в этот момент

    public HostelUIPanel[] uiHostelPanels;     //массив панелей, которые нужно обновить меню хостелов при старте анимации

    public Animator[] uiAnimators;      //МАССИВ ВСЕХ АНИМАЦИЙ , КРОМЕ TAP FIELD
    // 0 - меню хостелов
    // 1 - их апгрейды
    // 2 - main menu
    // 3 - ачивки
    // 4 - boosts
    // 5 - research
    // 6 - levels screen
    // 7 - store screen
    // 8 - bus station screen
    // 9 - recording studio
    // 10 - post box notif
    // 11 - capacity notif
    // 12 - buy bus screen
    // 13 - afk notif
    // 14 - extra pers bonus    //mission complete
    // 15 - veh capacity notif
    // 16 - stats screen
    // 17 - prestige screen
    // 18 - settings screen
    // 19 - watch ad notif
    // 20 - daily gift
    // 21 - leaderboard
    // 22 - about screen
    // 23 - snapshotscreen

    [Header("NOTIF PREFABS")]           //префабы для контейнера уведомлений
    public GameObject container;        //сам контейнер

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;      //выкл затухание экрана
    }

    public void SetFieldAnim()              //set buttons anim
    {
        if (setFieldState == 0 || setFieldState == 2)
        {
            setFieldState = 1;
            minTapZone.SetActive(false);
            maxTapZone.SetActive(true);
            //spawnButton.interactable = false;
        }
        else
        {
            setFieldState = 2;
            minTapZone.SetActive(true);
            maxTapZone.SetActive(false);
            //spawnButton.interactable = true;
        }

        setTapField.SetInteger("State", setFieldState);
        fiveButtons.SetInteger("State", setFieldState);
    }

    public void HideHostelsBtn()            //скрыть меню с всеми хостелами
    {
        HideAllScreens(uiAnimators[1]);
    }

    public void ShowHostelsBtn()            //показать меню хоста
    {
        //print("getTouch");
        //if (hostelUpdState != 1)
        //{
            HideAllScreens(uiAnimators[0]);
            uiAnimators[0].SetInteger("State", 1);
        foreach (HostelUIPanel _ui in uiHostelPanels)
        {
            _ui.UpdateData();
        }
        //}
    }

    public void ShowBusStationMenu(int _i)            //открыть меню станции
    {
        HideAllScreens(uiAnimators[8]);
        uiAnimators[8].SetInteger("State", _i);
    }

    public void SetHostelUpdAnim()          //set hostel upgrades anim
    {
        if (hostelUpdState == 0 || hostelUpdState == 2)
        {
            hostelUpdState = 1;
            uiAnimators[0].SetInteger("State", 2);
        }
        else
        {
            hostelUpdState = 2;
            uiAnimators[0].SetInteger("State", 1);
        }

        uiAnimators[1].SetInteger("State",hostelUpdState);
    }

    public void ShowMainMeni(int _i)        //MAIN MENU SCREEN
    {
        HideAllScreens(uiAnimators[2]);

        if (_i == 1 && uiAnimators[2].GetInteger("State") == 1)
        {
            uiAnimators[2].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[2].SetInteger("State", _i);
        }
    }

    public void HideAllScreens(Animator _current)           //СКРЫВАЕТ ВСЕ СКИНЫ КРОМЕ ПАРАМЕТРА
    {
        foreach (Animator _anim in uiAnimators)
        {
            if(_anim != _current)
                _anim.SetInteger("State",2);
        }
    }

    public void ShowAchievments(int _i)//МЕНЮ АЧИВОК//МЕНЮ АЧИВОК
    {
        HideAllScreens(uiAnimators[3]);

        if (_i == 1 && uiAnimators[3].GetInteger("State") == 1)
        {
            uiAnimators[3].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[3].SetInteger("State", _i);
        }
    }

    public void ShowBoosts(int _i)//МЕНЮ BOOSTS
    {
        HideAllScreens(uiAnimators[4]);

        if (_i == 1 && uiAnimators[4].GetInteger("State") == 1)
        {
            uiAnimators[4].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[4].SetInteger("State", _i);
        }
    }

    public void ShowResearch(int _i)//МЕНЮ Research
    {
        HideAllScreens(uiAnimators[5]);

        if (_i == 1 && uiAnimators[5].GetInteger("State") == 1)
        {
            uiAnimators[5].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[5].SetInteger("State", _i);
        }
    }

    public void ShowLevelScreen(int _i)//МЕНЮ Levels
    {
        HideAllScreens(uiAnimators[6]);

        if (_i == 1 && uiAnimators[6].GetInteger("State") == 1)
        {
            uiAnimators[6].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[6].SetInteger("State", _i);
        }
    }

    public void ShowStoreScreen(int _i)//МЕНЮ Levels
    {
        HideAllScreens(uiAnimators[7]);

        if (_i == 1 && uiAnimators[7].GetInteger("State") == 1)
        {
            uiAnimators[7].SetInteger("State", 2);
        }
        else
        {
            uiAnimators[7].SetInteger("State", _i);
        }
    }

    public void ShowRecordingScreen(int _i) //меню recording building
    {
        HideAllScreens(uiAnimators[9]);
        uiAnimators[9].SetInteger("State", _i);
    }

    public void ShowPostNotifScreen(int _i)     //screen post box notif
    {
        HideAllScreens(uiAnimators[10]);
        uiAnimators[10].SetInteger("State", _i);
    }

    public void ShowCapacityNotifScreen(int _i)     //capacity screen
    {
        HideAllScreens(uiAnimators[11]);
        uiAnimators[11].SetInteger("State", _i);
    }

    public void ShowBuyBuScreen(int _i)     //buy bus screen
    {
        HideAllScreens(uiAnimators[12]);
        uiAnimators[12].SetInteger("State", _i);
        if (_i == 2)
        {
            uiAnimators[8].SetInteger("State",1);
        }
    }

    public void ShowAfkScreen(int _i)       //afk screen
    {
        HideAllScreens(uiAnimators[13]);
        uiAnimators[13].SetInteger("State", _i);
    }

    public void ShowExtraPersScreen(int _i)     //extra pers screen
    {
        HideAllScreens(uiAnimators[14]);
        uiAnimators[14].SetInteger("State", _i);
    }

    public void ShowVehCapacityNotif(int _i)        //not enaugh veh capacity
    {
        HideAllScreens(uiAnimators[15]);
        uiAnimators[15].SetInteger("State", _i);
    }

    public void ShowStatsScreen(int _i)        //stats screen
    {
        HideAllScreens(uiAnimators[16]);
        uiAnimators[16].SetInteger("State", _i);

        if (_i == 2)
            ShowMainMeni(1);
    }

    public void ShowPrestigeScreen(int _i)        //prestige screen
    {
        HideAllScreens(uiAnimators[17]);
        uiAnimators[17].SetInteger("State", _i);

        if (_i == 2)
            ShowMainMeni(1);
    }

    public void ShowSettingsScreen(int _i)        //settings screen
    {
        HideAllScreens(uiAnimators[18]);
        uiAnimators[18].SetInteger("State", _i);

        if (_i == 2)
            ShowMainMeni(1);
    }

    public void ShowADScreen(int _i)        //ad notif screen screen
    {
        HideAllScreens(uiAnimators[19]);
        uiAnimators[19].SetInteger("State", _i);
    }

    public void ShowDailyGiftScreen(int _i)        //daily gift screen
    {
        HideAllScreens(uiAnimators[20]);
        uiAnimators[20].SetInteger("State", _i);
    }

    public void ShowLeadScreen(int _i)        //leaderboard screen
    {
        HideAllScreens(uiAnimators[21]);
        uiAnimators[21].SetInteger("State", _i);

        if (_i == 2)
            ShowMainMeni(1);
    }

    public void ShowAboutScreen(int _i)        //about screen
    {
        HideAllScreens(uiAnimators[22]);
        uiAnimators[22].SetInteger("State", _i);

        if (_i == 2)
            ShowMainMeni(1);
    }

    public void ShowSnapScreen(int _i)        //snapshot screen
    {
        HideAllScreens(uiAnimators[23]);
        uiAnimators[23].SetInteger("State", _i);
    }

    void Update()       //скрытие окна по нажатию на esc
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiAnimators[0].SetInteger("State", 2);
            HideAllScreens(uiAnimators[0]);
        }
    }

    void OnGUI()            //СЧЕТЧИК ФПС
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
    
    /*public void CheckFps()
    {
        if ((int)fps <= 30 && SaveManager.SharedInstance.shadowToggle.isOn)
        {
            print("set low fps");
            SaveManager.SharedInstance.shadowToggle.isOn = false;
            SaveManager.SharedInstance.SetupShadows();
        }
    }*/
}
