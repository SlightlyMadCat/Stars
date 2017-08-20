using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarHostel : MonoBehaviour {
    public int hostelCapacity;          //общая вместимость хостела
    public int currentCapacity = 0;         //текущая загруженность
    CloneCenter cloneCenter;
    public GameObject[] hostelBuildings;            //префабы развития хостела
    public int currentHostelIndex;              //текущий индекс префаба
    public Image thisHostelCapacityScale;           //ИСКЛЮЧЕНИЕ - ССЫЛКА НЕ В UI MANAGER, ПОЛОСКА ЗАПОЛНЕНИЯ ХОСТЕЛА
    public bool isBuilded;          //построенко ли зданий - для распознавания нажатий

    public GameObject hostelPanel;
    public bool visible = false;

    private void Start()
    {
        cloneCenter = CloneCenter.SharedInstance;
    }

    private void FixedUpdate()
    {
        if (hostelPanel.transform.position.x < 700 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            UpdHostelScaleCapacity();
        }
        else if (hostelPanel.transform.position.x > 700 && visible == true)
        {
            visible = false;
        }
    }

    public void SetHostelPrefab(int _newI)          //апдейт модели хостела
    {
        if(cloneCenter == null)
            cloneCenter = GameObject.FindGameObjectWithTag("CloneCenter").GetComponent<CloneCenter>();

        if (_newI != -1 && _newI >= currentHostelIndex)
        {
            if (currentHostelIndex >= 0 && currentHostelIndex != _newI)
            {
                cloneCenter.UpdAvailableSlots(hostelBuildings[currentHostelIndex].GetComponent<CurrentHostel>().thisHostelCapacity * -1);
            }

            cloneCenter.UpdAvailableSlots(hostelBuildings[_newI].GetComponent<CurrentHostel>().thisHostelCapacity);
        }

        if ( _newI >= 0 && _newI < hostelBuildings.Length)
        {
            if(currentHostelIndex != -1) 
                hostelBuildings[currentHostelIndex].SetActive(false);
            currentHostelIndex = _newI;

            if (currentHostelIndex != -1)
            {
                isBuilded = true;
                hostelBuildings[currentHostelIndex].SetActive(true);
            }
        }
    }

    public void UpdCapacity()           //когда клон попадает в хостел
    {
        if (hostelCapacity > currentCapacity)
        {
            currentCapacity++;
            UpdHostelScaleCapacity();
        }
    }

    public void UpdHostelScaleCapacity()                   //АПД В ПОЛОСКИ ЗАПОЛНЕНИЯ ХОСТЕЛА В МЕНЮ
    {
        if (visible)
        {
            ImmidUpd();
        }
    }

    public void ImmidUpd()
    {
        float k = (currentCapacity * 1f) / (hostelCapacity * 1f);
        thisHostelCapacityScale.fillAmount = k;
        thisHostelCapacityScale.color = Color.Lerp(Color.green, Color.red, k);
    }
}
