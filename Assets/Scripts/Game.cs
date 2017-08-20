using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Game               //КЛАСС ДЛЯ ВСЕХ СОХРАНЕНИЙ В ИГРЕ
{
    public List<int> hostelIndex;      //список индексов префабов хостелов
    public List<int> clonesInHostel;        //список кол-ва мобов в домах 
    public double totalMoneyCount = 0;           //все деньги в игре
    public List<float> researchProgres;       //прогресс исследований
    public int goldCoins = 0;               //золотая валюта ?? нужно ли оставить в инт, или первести в бигИнт
    public List<int> carIndexes;            //список индексов машин
    public int carPlaceBought = 0;      //сколько слотов для машин куплено
    public DateTime exitTime;        //время выхода из игры
    public DateTime startBoostTime;      //время когда включился х2 буст
    public bool ifBoostActive = false;          //активирован ли этот буст на 2 часа
    public int curLevel = 0;                //текущий уровень
    public int droneTakesDown = 0;          //сколько дронов сбито
    public int extraPersecFound = 0;            //сколько экстра персов найдено
    public double totalMoneyOnAllLevels = 0;    //деньги за все уровни
    public int currentMissionNum = 0;           //номер текущей миссии

    //RENDERING QUALITY
    public bool fog = true;                    //on off fog
    public bool sounds = true;                     //звуки окружения
    public bool music = true;                      //музыка
    public bool softShados = false;                 //softness shdows
    public int timeSet = 0;                     //часть дня
    public int shadowQuality = 1;               //shadow quality
    public float shadowOpacity = 0f;
    public float shadowBlur = 0f;

    public int silosCount = 1;                  //сколько вышек куплено для афк
    public double productionMade = 0;                  //сколько сделано продукции этого типа
    public DateTime startDailyGiftTime;             //точка отсчета бонуса за 24 часа
    public int totalGoldCoins = 0;                      //колво всех золотых монет в копилке
    public int languageInt = 0;                     //индекс текущего языка
    public double totalInGameMinutes = 0;           //общее кол-во минут в игре
    public double totalEarnedMoney = 0;             //сколько денег заработано за всю игру
}
