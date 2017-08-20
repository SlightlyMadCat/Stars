using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CrowdTouch : MonoBehaviour {

    public bool isTextMessage;
    public GameObject parent;
    public GameObject crowd;
    public PeopleSpawner spawner;

    private Economics economics;
    bool trySpawn = false;
    //private UIManager ui;
    //private double extraPrize;

    public GameObject moneyGetText;

    private void OnEnable()
    {
        trySpawn = false;
    }

    private void Start()
    {
        economics = Economics.SharedInstance;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if(isTextMessage)
        {
            trySpawn = true;
            parent.GetComponent<CrowdTouch>().SpawnCloneInParent();
        }
    }

    private void OnDisable()
    {
        if (!isTextMessage)
            return;

        if(spawner)
            spawner.StartCor();
        crowd.GetComponent<CrowdPart>().message = null;

        if (trySpawn)
            CalculateExtra();
    }

    void CalculateExtra()
    {
        float i = Random.Range(0.5f, 3);         //вместо 10 - добавить прокачку
        double curPrize = economics.moneyPerSecond * i;      //получаем процент от FV

        if (economics.moneyPerSecond == 0)
            curPrize = Random.Range(0.5f, 3) * 100;

        GameObject text = Instantiate(moneyGetText, transform.position, moneyGetText.transform.rotation);
        //text.text = economics.getShortIndex(curPrize) + " $";
        text.GetComponentInChildren<TextMeshPro>().text = economics.getShortIndex(curPrize) + " $";

        //print(economics.getShortIndex(curPrize));

        Destroy(text.gameObject, 0.5f);

        economics.ImmidUpd(curPrize);

        SaveManager.SharedInstance.extraPersFound++;       //сохран

        if (Stats.SharedStats.visible)
            Stats.SharedStats.extraStarsFound.text = SaveManager.SharedInstance.extraPersFound + "";
    }

    public void SpawnCloneInParent()
    {
        CloneCenter.SharedInstance.AttractPeople(1, gameObject.tag, transform);
    }

    /*private void SendExtra()
    {
        ui = UIManager.SharedInstance;
        ui.ShowExtraPersScreen(1);
        ui.uiAnimators[14].GetComponentInChildren<Button>().onClick.AddListener(() => GetBonusFromExtraPers());
        CalcExtraPrize();
        ui.uiAnimators[14].GetComponentInChildren<Text>().text = economics.getShortIndex(extraPrize) + " $";
    }

    void CalcExtraPrize()           //вычисляю сколько дать за крутого перса
    {
        economics = Economics.SharedInstance;

        int i = Random.Range(1, 10);         //вместо 10 - добавить прокачку
        extraPrize = economics.moneyPerSecond / i;      //получаем процент от FV

        if (extraPrize == 0)
            extraPrize = Random.Range(10, 20) * 100;
    }

    public void GetBonusFromExtraPers()     //сколько денег начислят от супер перса
    {
        ui.ShowExtraPersScreen(2);
        economics.ImmidUpd(extraPrize);
        SaveManager.SharedInstance.extraPersFound++;
    }*/
}
