using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostelUIPanel : MonoBehaviour {
    public int indexOfHostel;           //индекс хостела, к которому привязан этот ui
    UIManager uiManager;
    StarHostel starHostel;
    public UpgradesPanel updPanel;          //ссылка на ui с покупкой зданий
    public int currentHostelPrefabIndex;        //номер префаба, который сейчас построен
    public GameObject defaultScreen;            //если еще не один хостел не куплен
    public GameObject mainScreen;               //основной экран

    public Image icon;      //hostels icon
    public Text name;         //hostels name

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        starHostel = uiManager.allHostels[indexOfHostel].GetComponent<StarHostel>();
    }

    public void UpdateData()
    {
        currentHostelPrefabIndex = starHostel.currentHostelIndex;

        if (currentHostelPrefabIndex == -1)
        {
            mainScreen.SetActive(false);
        } else
        {
            icon.GetComponent<Image>().sprite = starHostel.hostelBuildings[currentHostelPrefabIndex].GetComponent<CurrentHostel>().icon;
            name.text = starHostel.hostelBuildings[currentHostelPrefabIndex].GetComponent<CurrentHostel>().name;

            defaultScreen.SetActive(false);
            mainScreen.SetActive(true);
        }
    }

    public void UpgradeThisHostel(int _i)           //апдейт новой модели хостела
    {
        starHostel.SetHostelPrefab(_i);
        currentHostelPrefabIndex = _i;

        UpdateData();
    }

    public void SetThisPanelToUpdPanel()            //ссылка в панелт прокачки зданий
    {
        currentHostelPrefabIndex = starHostel.currentHostelIndex;
        uiManager.SetHostelUpdAnim();
        updPanel.SetCurrentHostel(this);
    }
}
