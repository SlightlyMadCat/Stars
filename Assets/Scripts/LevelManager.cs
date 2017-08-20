using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelManager : MonoBehaviour {

    public static LevelManager SharedInstance;
    public GameObject snapshotNotif;

    public Economics economics;
    public SaveManager saveManager;

    public Image[] circles;     //миниатюры
    public Sprite[] levelIcons;      //иконки уровней
    public string[] descriptions;       //описания всех уровней

    public Sprite activeLvl;
    public Sprite darkLevel;

    public Button buyBtn;           //переход на лвл
    public Image icon;              //иконка уровня в меню
    public Image buttonIcon;        //иконка уровня на осн панели

    public double[] cloneValues;        //цена за одного клона по уровням
    public double[] valueToExplore;     //цена чтобы исследовать новый уровень
    public double[] valueToBuy;         //ценя для перехода на след лвл

    public int currentLevel = 0;        //текущий уровень
    public int swipeLvl = 0;

    public Text description;
    public Text fvText;

    public GameObject panel;        //ссылка на панель
    public bool visible = false;

    private void Awake()
    {
        SharedInstance = this;
    }

    public void SetupLevel(int _lvl)        //setup num
    {
        currentLevel = _lvl;
        ObjectPooler.SharedInstance.SetPool();

        swipeLvl = currentLevel;
        SwipeLvl(0);

        //economics.bigCloneValue = cloneValues[currentLevel];
    }

    private void FixedUpdate()
    {
        if (panel.transform.position.x < 600 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
        }
        else if (panel.transform.position.x > 600 && visible == true)
        {
            visible = false;
        }

        if (currentLevel+1 < valueToBuy.Length)         //проверка на кол-во денег
        {
            if (economics.totalMoney >= valueToBuy[currentLevel + 1])
            {
                if (!snapshotNotif.activeSelf)        //если уровень пройден - предложить сделать скриншот
                {
                    Application.CaptureScreenshot(ShareScreenshot.SharedInstance.ScreenshotName);
                    snapshotNotif.SetActive(true);
                }
            } else
            {
                if (snapshotNotif.activeSelf)
                {
                    snapshotNotif.SetActive(false);
                }
            }
        }
    }

    /*private void Start()
    {
        foreach (string _str in valueToBuy)
        {
            //print(economics.getShortIndex(new ScottGarland.BigInteger(_str)));
        }
    }*/

    public void SwipeLvl(int _val)          //переключение кнопок
    {
        //int swipeLvl = currentLevel;
        circles[swipeLvl].sprite = darkLevel;

        if (_val == 1) {
            if (swipeLvl < circles.Length - 1 ) {
                swipeLvl += 1;
            } else
            {
                swipeLvl = 0;
            }
        } else if (_val == -1)
        {
            if (swipeLvl > 0) {
                swipeLvl += -1;
            } else
            {
                swipeLvl = circles.Length-1;
            }
        }

        circles[swipeLvl].sprite = activeLvl;

        if (currentLevel < swipeLvl)
        {
            ChekInterectable(swipeLvl);
        }
        else if (currentLevel == swipeLvl)
        {
            buyBtn.GetComponentInChildren<Text>().text = "Current level";
            buyBtn.interactable = false;
        }
        else
        {
            buyBtn.GetComponentInChildren<Text>().text = "Previous level";
            buyBtn.interactable = false;
        }


        icon.GetComponent<Image>().sprite = levelIcons[swipeLvl];
        buttonIcon.GetComponent<Image>().sprite = levelIcons[currentLevel];

        description.text = descriptions[swipeLvl];

        if (cloneValues[swipeLvl] > 10)
            fvText.text = economics.getShortIndex(cloneValues[swipeLvl]) + " $";
        else
            fvText.text = cloneValues[swipeLvl] + " $";
    }

    public void ChekInterectable(int _i)           //проверка хватает ли денег на переход
    {
        //if (!visible)
        //    return;

        double _price = valueToBuy[_i];

        if (economics.totalMoney >= _price && currentLevel < swipeLvl)             //TODO добавить сюда не счетчек денег, а счетчик изученного и купленного
            buyBtn.interactable = true;
        else
            buyBtn.interactable = false;

        if (currentLevel < swipeLvl)
        {
            buyBtn.GetComponentInChildren<Text>().text = economics.getShortIndex(_price) + " $";
        }
    }

    public void SetNextLevel()          //переход на след уровень
    {
        currentLevel = swipeLvl;
        //economics.bigCloneValue = cloneValues[currentLevel];
        saveManager.GetNextLvl();
    }

    //TODO добавить отслеживание fv для открытия уровней
}
