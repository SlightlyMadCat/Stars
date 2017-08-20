using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostBox : MonoBehaviour {
    // public string money;             // доставка кэша - сделать рандомное значение
    public float minPercent;
    public float maxPercent;

    public GameObject notifPrefab;
    public bool isPostBox;
    public bool isCapacityNotif;
    public bool isVehNotif;

    public GameObject spawnedNotif;
    UIManager uiManager;
    Economics economics;
    double curPrize;       //сколько дропнется с коробки

    CarSpawner carSpawner;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        economics = GameObject.FindGameObjectWithTag("Economics").GetComponent<Economics>();

        if (isPostBox)
            SpawnNotif();
    }

    private void OnMouseDown()
    {
        if (isPostBox)
        {
            ShowUIScreenPostBox();
        }

        if (isCapacityNotif)
        {
            ShowUICapacityNotif();
        }

        if (isVehNotif)
        {
            ShowVehNotif();
        }
    }

    void CalculateMoney()
    {
        float i = Random.Range(minPercent, maxPercent);         //вместо 10 - добавить прокачку
        curPrize = economics.moneyPerSecond / i * 100;

        if (curPrize == 0)
            curPrize = Random.Range(minPercent, maxPercent) * 100;
        //economics.ImmidUpd(curPrize);
    }

    void SendMoney()
    {
        //CalculateMoney();
        //economics.ImmidUpd(new ScottGarland.BigInteger(money));
        economics.ImmidUpd(curPrize);
        carSpawner.canSpawn = true;
        Destroy(gameObject);
    }

    public void SpawnNotif()
    {
        carSpawner = GameObject.FindGameObjectWithTag("CarSpawner").GetComponent<CarSpawner>();
        carSpawner.canSpawn = false;
        uiManager = UIManager.SharedInstance;

        spawnedNotif = Instantiate(notifPrefab, uiManager.container.transform.position, notifPrefab.transform.rotation);
        spawnedNotif.transform.SetParent(uiManager.container.transform, true);
        spawnedNotif.transform.localScale = new Vector3(1,1,1);

        if(isPostBox)
            spawnedNotif.GetComponent<Button>().onClick.AddListener(() => ShowUIScreenPostBox());
        if(isCapacityNotif)
            spawnedNotif.GetComponent<Button>().onClick.AddListener(() => ShowUICapacityNotif());
    }

    public void HideNotif()
    {
        if (spawnedNotif != null)
            Destroy(spawnedNotif);
    }

    public void ShowUIScreenPostBox()
    {
        uiManager = UIManager.SharedInstance;
        uiManager.ShowPostNotifScreen(1);
        CalculateMoney();
        uiManager.uiAnimators[10].gameObject.GetComponentInChildren<Text>().text = economics.getShortIndex(curPrize) + " $";

        HideNotif();
        SendMoney();
    }

    public void ShowUICapacityNotif()
    {
        uiManager = UIManager.SharedInstance;
        uiManager.ShowCapacityNotifScreen(1);
        //uiManager.uiAnimators[11].gameObject.GetComponentInChildren<Text>().text = money.ToString();

        HideNotif();
    }

    void ShowVehNotif()
    {
        uiManager = UIManager.SharedInstance;
        uiManager.ShowVehCapacityNotif(1);
    }
}
