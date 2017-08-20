using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPanel : MonoBehaviour {
    public HostelUIPanel currentHostel;
    public GameObject[] allHostelBuyPanels;
    public Scrollbar vertBar;

    public bool visible = false;       //панель в поле зрения камеры

    public void SetCurrentHostel(HostelUIPanel _panel)          //инициализация
    {
        currentHostel = _panel;
        SetActiveButtons();

        vertBar.value = 1;      //для того чтобы список всех зданий отображался сверху
    } 

    public void SendUpdate(int _i)          //отправка в центр зданий нвого префаба
    {
        currentHostel.UpgradeThisHostel(_i);
        SetActiveButtons();
    }

    public void RestartUIButtons()      //рестарт массива при hide ui
    {
        foreach (GameObject _go in allHostelBuyPanels)
        {
            _go.SetActive(true);
        }
    }

    public void SetActiveButtons()          //dsвыбор домов которые можно купить
    {
        if (currentHostel == null)
            return;

        for (int i = 0; i < allHostelBuyPanels.Length; i++)
        {
            if (currentHostel.currentHostelPrefabIndex >= i)
            {
                allHostelBuyPanels[i].SetActive(false);
            } else
            {
                allHostelBuyPanels[i].GetComponent<BuyHostelPanel>().SetHostelTTX(i);       //ОБНОВЛЯЮ ТТХ
                allHostelBuyPanels[i].SetActive(true);
            }
        }
    }

    public void CheckButtons()
    {
        //if (currentHostel == null || GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().uiAnimators[1].GetInteger("State") != 1)
        //    return;

        if (!visible)
            return;

        if (!currentHostel)
            return;

        for (int i = 0; i < allHostelBuyPanels.Length; i++)
        {
            if (currentHostel.currentHostelPrefabIndex >= i)
            {
                //allHostelBuyPanels[i].SetActive(false);
            }
            else
            {
                //print("ff");
                allHostelBuyPanels[i].GetComponent<BuyHostelPanel>().CheckIfInterectable(GameObject.FindGameObjectWithTag("Hostel").GetComponent<StarHostel>().hostelBuildings[i].GetComponent<CurrentHostel>());       //ОБНОВЛЯЮ ТТХ
                //allHostelBuyPanels[i].SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.x < 600 && !visible)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            SetActiveButtons();
            vertBar.value = 1;
        }
        else if(transform.position.x > 600 && visible)// панель выходит из поля зрения камеры
        {
            visible = false;
        }
    }
}
