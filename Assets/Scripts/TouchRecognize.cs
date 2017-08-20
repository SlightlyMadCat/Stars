using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchRecognize : MonoBehaviour {

    public CloneCenter cloneCenter;
    public UIManager uiManager;
    public Text testTap;                //дебаг кол-ва нажатий
    List<Vector3> tapPos = new List<Vector3>();         //список точек нажатия - чтобы не дублировалось при одиночном тапе
    //public float spawnDelay;        //промежуток между спауном - при продолжительном нажатии
    //float curDelay = 0;
    //bool canSpawn = false;
    //float numTaps = 0;
    public float addClonePerSec;
    //для распознавания нажатий на здания
    List<Vector3> startPos = new List<Vector3>();           //точка начала
    //public GameObject marker;

    private void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
                    for (int i = 0; i < Input.touches.Length; i++)
                    {
                        if (IsPointerOverUIObject())
                            return;

                        if(Input.touches[i].phase == TouchPhase.Began)
                        {
                            RaycastHit hitInfo;
                            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(i).position), out hitInfo, Mathf.Infinity);

                            if (hitInfo.transform.tag == "Person" || hitInfo.transform.tag == "Person2" || hitInfo.transform.tag == "Person3" || hitInfo.transform.tag == "Person4" || hitInfo.transform.tag == "Person5")
                            {
                                hitInfo.transform.GetComponent<CrowdTouch>().SpawnCloneInParent();
                            }
                        }
                    }


            //РАСПОЗНОВАНИЕ НАЖАТИЙ НА ЗДАНИЯ

            foreach (Touch _touch in Input.touches)
            {
                if (IsPointerOverUIObject())
                    return;
                if (_touch.phase == TouchPhase.Began /*|| _touch.phase == TouchPhase.Moved*/)
                {
                    startPos.Add(_touch.position);
                } else if (_touch.phase == TouchPhase.Ended)
                {
                    if (startPos.Contains(_touch.position))
                    {
                        BuildingRecognize(_touch);
                    } else
                    {
                        float dist = Mathf.Infinity;

                        foreach (Vector3 _oldPos in startPos)
                        {
                            if(Vector3.Distance(_touch.position, _oldPos) < dist)
                            {
                                dist = Vector3.Distance(_touch.position, _oldPos);
                            }
                        }

                        if (dist < 45)
                            BuildingRecognize(_touch);
                    }
                }
            }
        }
    }

    void BuildingRecognize(Touch _touch)
    {

        if (Input.touchCount > 2)
            return;

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(_touch.position), out hitInfo, Mathf.Infinity);

        if (hit)
        {
            int id = _touch.fingerId;
            if (IsPointerOverUIObject())                //блок при нажатии на ui
            {
                return;
                // ui touched
            }

            switch (hitInfo.transform.tag)
            {
                case "Hostel":
                    if (hitInfo.transform.GetComponent<StarHostel>().isBuilded) {
                        uiManager.ShowHostelsBtn();
                    }
                    break;
                case "BusStation":
                    uiManager.ShowBusStationMenu(1);
                    break;
            }

            switch (hitInfo.transform.name)
            {
                case "Surgery":
                    SwitchResScreen(0);
                    break;
                case "Barber":
                    SwitchResScreen(2);
                    break;
                case "Cosmetics":
                    SwitchResScreen(3);
                    break;
                case "GYM":
                    SwitchResScreen(1);
                    break;
                case "RecordingStudio":
                    uiManager.ShowRecordingScreen(1);
                    break;
            }
        }

        startPos.Remove(_touch.position);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void SwitchResScreen(int i)     //выбираю, какой из экранов исследований показать
    {
        for (int j = 0; j < ResearchContainer.SharedInstance.containers.Length; j++)
        {
            if (i != j)
                ResearchContainer.SharedInstance.containers[j].SetActive(false);
            else
                ResearchContainer.SharedInstance.containers[j].SetActive(true);
        }

        uiManager.ShowResearch(1);
        ResearchContainer.SharedInstance.scroll.value = 1;
    }
}
