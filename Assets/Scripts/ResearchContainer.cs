using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchContainer : MonoBehaviour {

    public static ResearchContainer SharedInstance;

    public CurrentResearch[] allResearches;
    public GameObject[] tierInfos;
    public Scrollbar scroll;
    public float allResCounter = 0;

    public int[] resIndexes;
    public Image[] progressBars;
    public Sprite doneImage;

    public GameObject[] containers;

    public bool visible = false;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        scroll.GetComponent<Scrollbar>().value = 1;     //всегда вверх
    }

    private void FixedUpdate()
    {
        if (transform.position.x < 600)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            //CheckButtons();
        }
        else // панель выходит из поля зрения камеры
        {
            visible = false;
        }

        /*bool shouldShow = Mathf.Abs(scrollRect.InverseTransformPoint(t.position).y) < scrollRect.rect.height;
        if (shouldShow != childTransform.gameObject.activeSelf)
        {
            childTransform.gameObject.SetActive(shouldShow);
        }*/
    }

    public void SetActiveBtnsResearch()
    {
        //if (!visible)
        //    return;

        foreach (CurrentResearch _panel in allResearches)
        {
            _panel.CalculatePrice();
        }
        //SetTierValue(false);
    }

    public void SetTierValue(bool _must)
    {
        if (GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().uiAnimators[5].GetInteger("State") != 1)        //если меню скрыто, не обновлять
        {
            if(_must == false)
                return;
        }

        //print("f");
        int i = 0;
        foreach (GameObject _tier in tierInfos)
        {
            float f = resIndexes[i] - allResCounter;
            if (f > 0)
            {
                _tier.GetComponentInChildren<Text>().text = f + "";
                progressBars[i].GetComponent<Image>().fillAmount = allResCounter / resIndexes[i]*1f;
            } else
            {
                _tier.GetComponentInChildren<Text>().text = "";
                progressBars[i].GetComponent<Image>().fillAmount = 1;
            }
            i++;
        }
    }
}
