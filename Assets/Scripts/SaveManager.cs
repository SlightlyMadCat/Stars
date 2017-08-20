using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using GameSparks.Core;

public class SaveManager : MonoBehaviour {
    //спислк всех хостелов для сохранов и ля load math
    [HideInInspector]
    public GameObject[] hostels;

    public static SaveManager SharedInstance;
    public Light lightOnScene;

    [Header("SETTINGS TOGGLES")]
    public Toggle shadowToggle;
    public Toggle soundsToggle;
    public Toggle musicToggle;
    public Toggle softShadowToggle;

    public Dropdown timeSetDrop;
    public Dropdown qualityDrop;
    public Dropdown languageDrop;

    public Slider opacitySlider;
    public Slider blurSlider;

    public Game gameSave;
    public Game gameLoad;
    public bool dataReset;      //удаление сохрана
    //bool changeToNextLvl = false;       //если вкл переход на след лвл

    public Text debug;
    public Text playerID;
    CloneCenter cloneCenter;
    public ChangeScene changeScene;
    public Economics economics;
    public ResearchContainer research;
    public BusScreenCont busScreen;
    public RecordingStudio recStdio;
    public BoostManager boostManager;
    public LevelManager lvlManager;
    public GameSparksManager gsManager;
    public DemoControl demoControl;
    public Missions missions;

    //для статки
    public int dronesTakeDown = 0;
    public int extraPersFound = 0;
    public System.DateTime startSessionTime;        //время начала сессии

    private void Awake()
    {
        SharedInstance = this;
        startSessionTime = System.DateTime.Now;
    }

    private void Start()
    {
        cloneCenter = CloneCenter.SharedInstance;
        hostels = GameObject.FindGameObjectsWithTag("Hostel");

        gameLoad = new Game();
        gameLoad.totalMoneyOnAllLevels = 0;

        LoadData();

        gameSave = new Game();

        gameSave.hostelIndex = new List<int>();
        gameSave.clonesInHostel = new List<int>();
        gameSave.researchProgres = new List<float>();
        gameSave.carIndexes = new List<int>();

        LoadDataFromCloud();
        SaveData();
    }

    void DataToCloud()
    {
        gsManager.SendDataToCloud();
        //LoadDataFromCloud();
    }

    public void LoadDataFromCloud()
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("LOAD_PLAYER").Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Received Player Data From GameSparks...");
                GSData data = response.ScriptData.GetGSData("player_Data");
                //print("Player ID: " + data.GetString("playerID"));
                playerID.text = data.GetString("playerID");
                //print("Player money: " + data.GetString("playerMoney"));
                //print("Player clones: " + data.GetInt("playerClones"));
                //print("Player level: " + data.GetInt("playerLevel"));
                //print("Player exit time: " + data.GetLong("playerExitTime"));
            }
            else
            {
                playerID.text = "ERROR";
                Debug.Log("Error Loading Player Data...");
            }
        });
    }

    private void OnApplicationQuit()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            print("editor");
            SaveData();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            print("vedro");
            SaveData();
        }
    }

    public void SaveData()
    {
        GetHostelIndexes();
        //SaveLoad.savedGames.Add(Game.current);
        BinaryFormatter bf = new BinaryFormatter();
        //print(Application.persistentDataPath);
        //Application.persistentDataPath это строка; выведите ее в логах и вы увидите расположение файла сохранений
        FileStream file = File.Create(Application.persistentDataPath + "/test.af");
        bf.Serialize(file, gameSave);
        file.Close();
        DataToCloud();
    }

    public void DataReset()
    {
        dataReset = true;
        SaveData();
        changeScene.LoadScene();
    }

    public void GetNextLvl()
    {
        dataReset = true;
        //changeToNextLvl = true;
        SaveData();
        changeScene.LoadScene();
    }

    void GetHostelIndexes()         //сохраняю список все индексов хостелов  и остальных данныъ
    {
        gameSave = new Game();

        gameSave.hostelIndex = new List<int>();
        gameSave.clonesInHostel = new List<int>();
        gameSave.researchProgres = new List<float>();
        gameSave.carIndexes = new List<int>();

        if (dataReset == false)
        {
//////////////////////////////////////////////
            gameSave.totalInGameMinutes = SessionTime();
//////////////////////////////////////////////
            for(int i = 0; i < hostels.Length; i++)         
            {
                gameSave.hostelIndex.Add(hostels[i].GetComponent<StarHostel>().currentHostelIndex);
                gameSave.clonesInHostel.Add(hostels[i].GetComponent<StarHostel>().currentCapacity);
            }

            gameSave.totalMoneyCount = economics.totalMoney;
            gameSave.totalMoneyOnAllLevels = economics.allTimeMiney;

            gameSave.goldCoins = economics.goldCoins;
            gameSave.totalGoldCoins = economics.totalGoldCoins;
            gameSave.carIndexes = busScreen.carIndexes;
            gameSave.carPlaceBought = busScreen.currentNumPlaces;
            gameSave.exitTime = System.DateTime.Now;
            //gameSave.productionMade = economics.prodMade;

            gameSave.startBoostTime = boostManager.startDoubleBoost;
            gameSave.ifBoostActive = boostManager.boostTimer;

            gameSave.curLevel = lvlManager.currentLevel;

            foreach (CurrentResearch _currentRes in research.allResearches)
            {
                gameSave.researchProgres.Add(_currentRes.currentValue);
            }

            gameSave.droneTakesDown = dronesTakeDown;
            gameSave.extraPersecFound = extraPersFound;

            double moneyPerMatch = economics.totalMoney - gameLoad.totalMoneyOnAllLevels;//разница заработанных денег

            gameSave.totalMoneyOnAllLevels = moneyPerMatch + gameLoad.totalMoneyOnAllLevels;       //сумма за все пред уровни и за этот
            gameSave.startDailyGiftTime = economics.startDailyGiftTime;
        } else
        {
            //gameSave.currentMissionNum = 0;
            //////////////////////////////////////////////
            gameSave.totalInGameMinutes = SessionTime();
            //////////////////////////////////////////////

            for (int i = 0; i < 4; i++)         //сброс данных про префаб хостела и вместимость
            {
                gameSave.hostelIndex = new List<int>() { -1, -1, -1, 0 };
                gameSave.clonesInHostel.Add(0);
            }

            gameSave.totalMoneyCount = 0;
            gameSave.totalMoneyOnAllLevels = 0;         //ПРОВЕРИТЬ СБРОС К 0

            gameSave.goldCoins = economics.goldCoins;
            gameSave.totalGoldCoins = economics.totalGoldCoins;
            gameSave.carIndexes = new List<int>() {0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
            gameSave.carPlaceBought = BusStation.SharedInstance.carPrefabs.Length;        //wtf
            gameSave.exitTime = System.DateTime.Now;
            //gameSave.productionMade = 0;

            gameSave.startBoostTime = System.DateTime.Now;
            gameSave.ifBoostActive = false;

                //gameSave.goldCoins = economics.goldCoins;
                gameSave.curLevel = lvlManager.currentLevel;

                double moneyPerMatch = economics.totalMoney - gameLoad.totalMoneyOnAllLevels;//разница заработанных денег

                gameSave.totalMoneyOnAllLevels = moneyPerMatch + gameLoad.totalMoneyOnAllLevels;       //сумма за все пред уровни и за этот
                                                                                                       //gameSave.totalGoldCoins = economics.totalGoldCoins;

            /*for (int i = 0; i < 47; i++)            //сброс данных про исследования
            {
                gameSave.researchProgres.Add(0);
            }*/

            foreach (CurrentResearch _currentRes in research.allResearches)
            {
                gameSave.researchProgres.Add(0);
            }

            gameSave.droneTakesDown = 0;
            gameSave.extraPersecFound = 0;
            gameSave.startDailyGiftTime = economics.startDailyGiftTime;
        }

        gameSave.languageInt = languageDrop.value;

        gameSave.fog = shadowToggle.isOn;       //ТЕНИ
        gameSave.sounds = soundsToggle.isOn;
        gameSave.music = musicToggle.isOn;
        gameSave.softShados = softShadowToggle.isOn;
        gameSave.shadowQuality = qualityDrop.value;
        gameSave.timeSet = timeSetDrop.value;
        gameSave.shadowBlur = blurSlider.value;
        gameSave.shadowOpacity = opacitySlider.value;

        gameSave.silosCount = recStdio.silosCount;      //купленные за бабки вышки афк
        gameSave.currentMissionNum = missions.mission_number;
    }

    void SendReadHostelIndex()          //читаю данные хостелов         и все данные
    {
        /*lvlManager.SetupLevel(gameLoad.curLevel);
        gsManager.minutes = gameLoad.totalInGameMinutes;
        missions.mission_number = gameLoad.currentMissionNum;
        missions.Setup();

        languageDrop.value = gameLoad.languageInt;*/

        int i = 0;
        for(i = 0; i < hostels.Length; i++)
        {
            hostels[i].GetComponent<StarHostel>().SetHostelPrefab(gameLoad.hostelIndex[i]);        //номер префаба здания
            hostels[i].GetComponent<StarHostel>().currentCapacity = gameLoad.clonesInHostel[i];        //апд кол-во мобов в здании
            hostels[i].GetComponent<StarHostel>().ImmidUpd();           //апд полоса мобов

            cloneCenter.clonesNum += gameLoad.clonesInHostel[i];
        }

        cloneCenter.clonesSpawned = cloneCenter.clonesNum;

        lvlManager.SetupLevel(gameLoad.curLevel);
        gsManager.minutes = gameLoad.totalInGameMinutes;
        missions.mission_number = gameLoad.currentMissionNum;
        missions.Setup();

        languageDrop.value = gameLoad.languageInt;

        cloneCenter.capacityPanel = UIManager.SharedInstance.timeSpawnPanel;
        cloneCenter.UpdUIClonesNum();

        economics.startDailyGiftTime = gameLoad.startDailyGiftTime;
        economics.totalMoney = gameLoad.totalMoneyCount;            //апд экономики
        economics.goldCoins = gameLoad.goldCoins;
        economics.totalGoldCoins = gameLoad.totalGoldCoins;
        economics.moneyPerAllCycles = gameLoad.totalMoneyOnAllLevels;
        //economics.prodMade = gameLoad.productionMade;

        economics.Setup();

        busScreen.carIndexes = gameLoad.carIndexes;
        busScreen.currentNumPlaces = gameLoad.carPlaceBought;
        busScreen.SetSlots();       //вызываю заполение слготов 

        i = 0;
        ResearchContainer.SharedInstance.SetActiveBtnsResearch();
        foreach (CurrentResearch _currentRes in research.allResearches)         //АПД ПРОГРЕССА ИССЛЕДОВАНИЙ
        {
            _currentRes.GetSavedResearch(gameLoad.researchProgres[i]);
            i++;
        }

        recStdio.getSaveData(gameLoad.exitTime);

        if(gameLoad.ifBoostActive)
            boostManager.SetupBoost(gameLoad.startBoostTime);

        dronesTakeDown = gameLoad.droneTakesDown;
        extraPersFound = gameLoad.extraPersecFound;

        shadowToggle.isOn = gameLoad.fog;
        soundsToggle.isOn = gameLoad.sounds;
        musicToggle.isOn = gameLoad.music;
        softShadowToggle.isOn = gameLoad.softShados;
        timeSetDrop.value = gameLoad.timeSet;
        qualityDrop.value = gameLoad.shadowQuality;
        opacitySlider.value = gameLoad.shadowOpacity;
        blurSlider.value = gameLoad.shadowBlur;

        SetupShadows();
        SetupSounds();
        SetupMusic();
        SetupSoftShadows();
        SetupTimeSet();
        SetupShadowQuality();
        SetupBlur();
        SetupOpacity();

        recStdio.silosCount = gameLoad.silosCount;
        recStdio.UpdSilosCount(0);

        print("ALL DATA WELL DONE");
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/test.af"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/test.af", FileMode.Open);
            gameLoad = (Game)bf.Deserialize(file);
            file.Close();

            SendReadHostelIndex();
            //debug.text = "Save is loaded";
        } else
        {
            missions.mission_number = 0;
            missions.Setup();

            List<int> defaultIndex = new List<int>() { -1, -1, -1, 0 };
            int i = 0;
            for (i = 0; i < hostels.Length; i++)
            {
                hostels[i].GetComponent<StarHostel>().SetHostelPrefab(defaultIndex[i]);        //номер префаба здания
                hostels[i].GetComponent<StarHostel>().UpdHostelScaleCapacity();
                //i++;
            }

            ResearchContainer.SharedInstance.SetActiveBtnsResearch();
            foreach (CurrentResearch _currentRes in research.allResearches)         //АПД ПРОГРЕССА ИССЛЕДОВАНИЙ
            {
                _currentRes.GetSavedResearch(0);
            }

            economics.startDailyGiftTime = System.DateTime.Now.AddHours(24);

            busScreen.SetSlots();       //вызываю заполение слготов 
            economics.Setup();
            lvlManager.SetupLevel(0);

            recStdio.UpdSilosCount(0);
            recStdio.getSaveData(gameLoad.exitTime);
            SetupTimeSet();

            cloneCenter.capacityPanel = UIManager.SharedInstance.timeSpawnPanel;
            cloneCenter.UpdUIClonesNum();
            //debug.text = "data not ";
        }

        cloneCenter.ready = true;
        gsManager.SendBoardToCloud();       //при заходе в игру - оправка данных для таблицы
    }

    public double SessionTime()
    {
        double min = 0;

        System.TimeSpan delay = System.DateTime.Now - startSessionTime;         //сохран времени сессии

        if (delay.TotalMinutes >= 0)
            min = gameLoad.totalInGameMinutes + delay.TotalMinutes;
        else
            min = gameLoad.totalInGameMinutes;

        return min;
    }

    public void DeleteAndReload()
    {
        File.Delete(Application.persistentDataPath + "/test.af");
        changeScene.LoadScene();
    }

    /*/////////////////////SETUP SETTINGS TOGGLES\\\\\\\\\\\\\\\\\\\\*/
    public void SetupShadows()          //вкл или выкл тени
    {
        gameSave.fog = shadowToggle.isOn;
        demoControl.EnableFog(shadowToggle.isOn);
    }

    public void SetupSounds()
    {
        //todo
        gameSave.sounds = soundsToggle.isOn;
    }

    public void SetupMusic()
    {
        //todo

        if (musicToggle.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;

        gameSave.music = musicToggle.isOn;
    }

    public void setLanguageSave()
    {
        gameSave.languageInt = languageDrop.value;
    }

    public void SetupSoftShadows()
    {
        gameSave.softShados = softShadowToggle.isOn;
        demoControl.TogleSoftShadow(softShadowToggle.isOn);
    }

    public void SetupTimeSet()
    {
        gameSave.timeSet = timeSetDrop.value;
        demoControl.ChangeTimeOfDay(timeSetDrop.value);
    }

    public void SetupShadowQuality()
    {
        gameSave.shadowQuality = qualityDrop.value;
        demoControl.ChangeQuality(qualityDrop.value);
    }

    public void SetupOpacity()
    {
        gameSave.shadowOpacity = opacitySlider.value;
        demoControl.ChangeOpacity(opacitySlider.value);
    }

    public void SetupBlur()
    {
        gameSave.shadowBlur = blurSlider.value;
        demoControl.ChangeBlur(blurSlider.value);
    }
}
