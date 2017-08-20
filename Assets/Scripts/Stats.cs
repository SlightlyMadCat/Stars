using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {
    public static Stats SharedStats;
    public bool visible = false;

    [Header("TEXT FIELDS")]
    public Text population;
    public Text starValue;
    public Text soulStarBonus;
    public Text starMakingRate;
    public Text currentStarsMade;
    public Text droneTakeDowns;
    public Text extraStarsFound;
    public Text totalEarnings;
    public Text prestiges;

    private void Awake()
    {
        SharedStats = this;
    }

    private void FixedUpdate()
    {
        if (transform.position.x < 600 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            SetupData();
        }
        else if (transform.position.x > 600 && visible == true)
        {
            visible = false;
        }
    }

    void SetupData()        //обновить все данные при открытии окна
    {
        if(CloneCenter.SharedInstance.clonesNum > 0)
            population.text = Economics.SharedInstance.getShortIndex(CloneCenter.SharedInstance.clonesNum);
        starValue.text = Economics.SharedInstance.getShortIndex(LevelManager.SharedInstance.cloneValues[LevelManager.SharedInstance.currentLevel]);
        //soulStarBonus
        if(Economics.SharedInstance.moneyPerSecond > 0)
            starMakingRate.text = Economics.SharedInstance.getShortIndex(Economics.SharedInstance.moneyPerSecond);
        //currentStarsMade.text = Economics.SharedInstance.getShortIndex(Economics.SharedInstance.prodMade);
        droneTakeDowns.text = SaveManager.SharedInstance.dronesTakeDown + "";
        extraStarsFound.text = SaveManager.SharedInstance.extraPersFound + "";
        Economics.SharedInstance.SetAllMoneyText();
        //prestiges
    }
}
