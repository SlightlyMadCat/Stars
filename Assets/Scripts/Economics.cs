using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;

public class Economics : MonoBehaviour {

    public static Economics SharedInstance;

    public Text moneyText;         //куда выводить кол-во всех денег
    public Text moneyPerSecondText;        //куда выводить деньги за секунду 
    public Text goldEggsText;
    public Text globalCoinsBankText;

    public double totalMoney = 0;
    public double allTimeMiney = 0;

    public double moneyPerSecond = 0;
    public double moneyPerAllCycles = 0;
    public double soulClones = 1;
    public double prodMade = 0;

    public int goldCoins = 0;                           //ЗОЛОТАЯ ВАЛЮТА
    public int totalGoldCoins = 0;              //gold bank

    UIManager uiManager;
    GameObject[] allHostels;

    //ДЛЯ РАСЧЕТА ФОРМУЛЫ К
    public float numOfClones;          //кол-во всех клонов
    public float layingRate;            //сколько оскаров в секунду
    //public float soulOscarBonus;        //оскар бонус
    public float eggPerSecond = 0;            //сколько яий выходит за игровой тик
    float addToNextSecond = 0;              //сколько частей яйца перенсти на след секунду
    public System.DateTime startDaily;          //точка отсчета

    public LevelManager lvlManager;
    public UpgradesPanel updHostelPanel;
    public ResearchContainer resContainer;
    public CloneCenter cloneCenter;
    public BusScreenCont busCont;
    public float boostK = 1;            //коэф буста

    [Header("WATCH AD PACKAGE")]
    public GameObject watchADnotif;         //уведомление для просмотра рекламы
    double nextMoneyTime = Mathf.Infinity;           //время дос лед рекламы
    public float minADtimer;      //сколько сейчас времени для рекламы
    public float maxADtimer;

    public float minPercentAd;
    public float maxPercentAd;

    double moneyPerAD;          //сколько денег дадут за просмотр рекламы
    //TODO GOLD AND CASH
    public GameObject dailyGoldenGift;      //уведолмение 24 часа подарок
    public System.DateTime startDailyGiftTime;      //точка отсчета daily gift
    public int eggsPerDailyGift;        //сколько яиц за подарок

    private void Awake()
    {
        SharedInstance = this;
    }

    public void Setup()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        allHostels = GameObject.FindGameObjectsWithTag("Hostel");

        moneyText = uiManager.totalMoneyText;
        moneyPerSecondText = uiManager.moneyPerSecondText;
        //bigCloneValue = new ScottGarland.BigInteger(cloneValue.ToString());

        //currentCd = moneyCountCd;       //чтобы таймер работал сразу после запуска сцены
        UpdUI();

        //startTime = DateTime.Now;
       // nextMoneyTime = Random.Range(20, 60) * 100;
        globalCoinsBankText.text = "Gold bank : " + totalGoldCoins;
    }

    private void FixedUpdate()
    {       
        numOfClones = cloneCenter.clonesNum;
        eggPerSecond += numOfClones * layingRate + addToNextSecond;

        if (eggPerSecond >= 1)          //если готово большего одно яййа, апд экономики
        {
            UpdMoney();

            addToNextSecond = eggPerSecond - (int)(eggPerSecond);
            addToNextSecond = Mathf.Abs(addToNextSecond);
            eggPerSecond = 0;

            System.TimeSpan spanGift = System.DateTime.Now - startDailyGiftTime;
            if (spanGift.TotalHours >= 1 && !dailyGoldenGift.activeSelf)
            {
                dailyGoldenGift.SetActive(true);
            }
        }
        //таймер для рекламы и уведомлений

        /*if (currentADtimer >= nextMoneyTime && nextMoneyTime != 0)
        {
            GetADnotification();
            currentADtimer = 0;
        } else
        {
            currentADtimer++;
        }*/
    }

    IEnumerator WaitAndPrint()
    {
        while (true)
        {
            nextMoneyTime = Random.Range(minADtimer, maxADtimer);
            yield return new WaitForSeconds((float)nextMoneyTime);

            GetADnotification();
            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(WaitAndPrint());
    }

    public void UpdMoney()              //расчет сколько денег можно получить за этот тик
    {
        numOfClones = cloneCenter.clonesNum;

        double FV = CalculateFV();
        moneyPerSecond = System.Math.Round(FV, 0);

        totalMoney += moneyPerSecond;

        UpdUI();
        busCont.UpdScale();

        if (Stats.SharedStats.visible)
        {
            Stats.SharedStats.starValue.text = LevelManager.SharedInstance.cloneValues[LevelManager.SharedInstance.currentLevel] + "";
            Stats.SharedStats.starMakingRate.text = getShortIndex(moneyPerSecond);
            prodMade += (int)eggPerSecond;
            //Stats.SharedStats.currentStarsMade.text = getShortIndex(prodMade) + "";
            SetAllMoneyText();
        }
       // System.GC.Collect();
    }

    public double CalculateFV()
    {
        double cloneFactor = 250 * (lvlManager.currentLevel + 1);
        double soulFactor = soulClones * (0.1f + soulClones * 0.01f);
        double capacityFactor = 1 + 0.0001f * (cloneCenter.availableSlotsHostel - cloneCenter.clonesNum);

        soulFactor = 1;         //TODO SOUL EGGS

        //if (eggPerSecond < 1)
        //    eggPerSecond = 1;

        double FV = (cloneCenter.clonesNum * lvlManager.cloneValues[lvlManager.currentLevel] /* layingRate*/ * soulFactor * cloneFactor * capacityFactor * (int)eggPerSecond * boostK);
        return FV;
    }

    public void SetAllMoneyText()
    {
        if (moneyPerAllCycles > totalMoney)
            Stats.SharedStats.totalEarnings.text = getShortIndex(moneyPerAllCycles + totalMoney) + "";
        else
            Stats.SharedStats.totalEarnings.text = getShortIndex(totalMoney) + "";
    }

    void UpdUI()        //ДОБАВИТЬ УСЛОВНЫЕ ОБОЗНАЧЕНИЯ         -- обновление всего ui и кнопок покупки
    {
        if(uiManager == null)
            uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //print(totalMoney);
        if (totalMoney > 0)
            moneyText.text = getShortIndex(totalMoney);
        if(moneyPerSecond > 0)
            moneyPerSecondText.text = getShortIndex(moneyPerSecond) + " / sec";

        uiManager.goldenCoinText.text = goldCoins + "";         //сколько золотых монет

        //проверка можно ли купить здание или исследование            //TODO 15.07: проверки для откл работы в фоновом режиме

        if(updHostelPanel.visible)
            updHostelPanel.CheckButtons();

        if(resContainer.visible)
            resContainer.SetActiveBtnsResearch();

        if(busCont.visible)
            busCont.UpdAvailableBtn();
        
        if(lvlManager.visible)
            lvlManager.ChekInterectable(lvlManager.swipeLvl);

        if(RecordingStudio.SharedInstance.visible)
            RecordingStudio.SharedInstance.CheckBuyBtn();

        /*lvlManager.ChekInterectable(lvlManager.swipeLvl);
        busCont.UpdAvailableBtn();
        resContainer.SetActiveBtnsResearch();
        updHostelPanel.CheckButtons();
        RecordingStudio.SharedInstance.CheckBuyBtn();*/
    }

    public void ImmidUpd(double _income) //апдейт при получении почты и тд
    {
        totalMoney += _income;

        if (_income > 0)
            allTimeMiney += _income;
        UpdUI();
    }

    public void GoldenEggUpd(int _i)        //обновить кол-во золотых яиц
    {
        goldCoins += _i;

        if(_i > 0)          //добавляю в банк, если это бонус а не покупка
            totalGoldCoins += _i;

        goldEggsText.text = goldCoins + "";
        globalCoinsBankText.text = "Gold bank : "+totalGoldCoins;

        if(BoostManager.SharedInstance.visible)
            BoostManager.SharedInstance.CheckInteractBtn();
    }

    public string getShortIndex(double _num)           //смотрю какая длина числа и какое сокращение ему присвоить
    {
        if (_num <= 0)
            return "0";

        double numPointsD = System.Math.Log10(_num);
        int incomeLength = (int)System.Math.Truncate(numPointsD) + 1;      //узнаю длину числа

        int length = incomeLength;

        if (length > 6)
        {
            int _diff = incomeLength - 6;
            _num = _num / (System.Math.Pow(10, _diff));
        } else
        {
            _num = System.Math.Truncate(_num);
        }

        string number = _num + "";

        if (length > 0 && length <= 3)
        {
            number += " ";
        }
        else if (/*length >= 4 && */length <= 6)
        {
            number = getCutNum(number, " ", incomeLength);
        } else if (/*length >= 4 && */length <= 9)
        {
           // print("NUMBER" + number);
            number = getCutNum(number, "M", incomeLength);
        } else if (/*length >= 4 && */length <= 12)
        {
            number = getCutNum(number, "B", incomeLength);
        }
        else if (/*length >= 4 && */length <= 15)
        {
            number = getCutNum(number, "T", incomeLength);
        }
        else if (/*length >= 4 && */length <= 18)
        {
            number = getCutNum(number, "qu", incomeLength);
        }
        else if (/*length >= 4 && */length <= 21)
        {
            number = getCutNum(number, "Qi", incomeLength);
        }
        else if (/*length >= 4 && */length <= 24)
        {
            number = getCutNum(number, "ss", incomeLength);
        }
        else if (/*length >= 4 && */length <= 27)
        {
            number = getCutNum(number, "Sp", incomeLength);
        }
        else if (/*length >= 4 && */length <= 30)
        {
            number = getCutNum(number, "O", incomeLength);
        }
        else if (/*length >= 4 && */length <= 33)
        {
            number = getCutNum(number, "N", incomeLength);
        }
        else if (/*length >= 4 && */length <= 36)
        {
            number = getCutNum(number, "De", incomeLength);
        }
        else if (/*length >= 4 && */length <= 39)
        {
            number = getCutNum(number, "U", incomeLength);
        }
        else if (/*length >= 4 && */length <= 42)
        {
            number = getCutNum(number, "Du", incomeLength);
        }
        else if (/*length >= 4 && */length <= 45)
        {
            number = getCutNum(number, "Tr", incomeLength);
        }
        else if (/*length >= 4 && */length <= 48)
        {
            number = getCutNum(number, "Qu", incomeLength);
        }

        return number;
    }

    string getCutNum(string _num, string _index, int _length)        //на сколько нужно обрезать какой индекс добавить
    {
        int length = _length;
        int cutIndex = length;
        int dotIndex = length;

        if (length % 3 == 1)
        {
            cutIndex = 4;
            dotIndex = 1;
            //print("f1");
        }
        else if ((length % 2 == 0 || length % 2 == 1) && length % 3 != 0)
        {
            cutIndex = 5;
            dotIndex = 2;
            //print("f2");
        }
        else if (length % 3 == 0)
        {
            cutIndex = 6;
            dotIndex = 3;
            //print("f3");
        }

        _num = _num.Substring(0, cutIndex);       //обрезаю строку
        _num = _num.Insert(_num.Length, " ");           //пробел между цифрами и сиволом сокращения
        _num = _num.Insert(_num.Length, _index);        //добавляю вконце сокращение
        _num = _num.Insert(dotIndex, ",");          //запятая длу удобности чтения
        //print(_num);
        return _num;
    }

    /*///////////////////////////////UI PART\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\*/

    void GetADnotification()        //показать предложение для просмотра рекламы
    {
        watchADnotif.SetActive(true);
        nextMoneyTime = 0;
    }

    public void ShowAdNotif()       //при нажатии на иконку уведомления
    {
        if (moneyPerSecond != 0)
            moneyPerAD = Random.Range(minPercentAd, maxPercentAd) * moneyPerSecond;     //сколько денег
        else
            moneyPerAD = Random.Range(minPercentAd, maxPercentAd) * LevelManager.SharedInstance.cloneValues[LevelManager.SharedInstance.currentLevel];
        uiManager.uiAnimators[19].gameObject.GetComponentInChildren<Text>().text = getShortIndex(moneyPerAD) + "$";
        uiManager.ShowADScreen(1);
        watchADnotif.SetActive(false);
    }

    public void WatchVideoAndHide()     //скрыть и начислить деньги
    {
        ImmidUpd(moneyPerAD);
        JustHideAdNotif();
    }

    public void JustHideAdNotif()       //скрыть уведомление и убрать иконку
    {
        uiManager.ShowADScreen(2);      //скрыть скрин
        watchADnotif.SetActive(false);
        //nextMoneyTime = Random.Range(minADtimer, 60) * 100;
        //currentADtimer = 0;
    }

    public void ShowDailyGiftScreen()       //show daily screen
    {
        //TODO - CHANGE BORDERS
        eggsPerDailyGift = Random.Range(15,80);

        uiManager.uiAnimators[20].gameObject.GetComponentInChildren<Text>().text = eggsPerDailyGift+"";
        uiManager.ShowDailyGiftScreen(1);
    }

    public void AddGoldenEggsAndHide()          //начислить дневной бонус и скрыть
    {
        GoldenEggUpd(eggsPerDailyGift);
        uiManager.ShowDailyGiftScreen(2);
        startDailyGiftTime = System.DateTime.Now.AddHours(24);         //сколько часов до след подарка
        dailyGoldenGift.SetActive(false);
        print(startDailyGiftTime);
    }
}
