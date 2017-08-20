using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentResearch : MonoBehaviour {
    public Text nameResearch;       //название
    public Image progressBar;       //будет отображаться прогресс
    public double startPrice;        //начальная цена
    public float maxValue;          //макс значение исследования
    public float currentValue;      //текущее значение (сохраняется)
    public int valueToOpen;         //сколько нужно, чтобы открыть
    public Button buyBtn;           //кнопка покупки
    double currentPrice = 0;     //текущая цена апгрейда
    public Text koefText;       //отображение буста
    ResearchContainer resCont;

    private void Awake()
    {
        //TODO FORMULA
        currentPrice = startPrice;
        resCont = GetComponentInParent<ResearchContainer>();
    }

    public Economics economics;

    public void GetSavedResearch(float _f)          //получение сохрана
    {
        currentValue = _f;
        if(resCont == null)
            resCont = GetComponentInParent<ResearchContainer>();

        resCont.allResCounter += currentValue;        //сохран сколько уровней открыто
        SetProgressBar();
        resCont.SetTierValue(true);
    }

    public void CalculateIfAvailable()
    {
        //TODO
    }

    public void SetProgressBar()            //апдейт прогресс бара
    {
        progressBar.GetComponent<Image>().fillAmount = currentValue / maxValue;
        SetDoneSprite();

        CalculatePrice();
    }

    void SetDoneSprite()            //провека на то что прокачка ветки завершена
    {
        if (currentValue == maxValue)
        {
            buyBtn.GetComponentInChildren<Text>().text = "";
            buyBtn.GetComponent<Image>().sprite = resCont.doneImage;
            buyBtn.GetComponent<Button>().enabled = false;
        } else
        {
            buyBtn.interactable = true;
            //CalculatePrice();
        }
    }

    public void CalculatePrice()           //вычисление и отображение цены
    {
        //TODO RECALCULATE PRICE
        //currentPrice = new ScottGarland.BigInteger(startPrice);
        if(currentValue < maxValue)
            buyBtn.GetComponentInChildren<Text>().text = economics.getShortIndex(currentPrice) +"$";
        //TODO KOEF TEXT

        SetActiveBtn();
    } 

    public void SetActiveBtn()              //проверка хватает ли денег
    {
        if(currentPrice == 0)
            currentPrice = startPrice;

        if (economics.totalMoney < currentPrice)
            buyBtn.interactable = false;
        else
        {
            SetDoneSprite();
        }


        if (GetComponentInParent<ResearchContainer>()) {
            if (GetComponentInParent<ResearchContainer>().allResCounter < GetComponentInParent<ResearchContainer>().resIndexes[valueToOpen])
            {
                buyBtn.interactable = false;
            }
        }
    }

    public void GetUpgrade(int _i)          //сюда подключить кнопки с апгрейдами
    {
        if (currentValue < maxValue)
        {
            currentValue++;
            resCont.allResCounter++;
            resCont.SetTierValue(false);

            //economics.totalMoney -= currentPrice;
            economics.ImmidUpd(-currentPrice);
            economics.GoldenEggUpd(1);

            SetProgressBar();       //обновить после покупки прогресс бар и новую цену
        }
    }
}
