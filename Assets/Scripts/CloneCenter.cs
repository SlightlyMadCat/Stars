using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Pathfinding;

public class CloneCenter : MonoBehaviour {

    public static CloneCenter SharedInstance;
    public Economics economics;

    public GameObject currentClone;     //текущий клон - выбрать на улице
    public Transform spawnPlace;            //место для спауна клона
    public Text clonesText;                 //ui
    public int clonesSpawned = 0;               //число клонов, которые заспаунились
    public int clonesNum = 0;                   //клоны во всех хотселах
    public int availableSlotsHostel;        //число свободных мест в хостелах

    Image timePanel;                 //полоска времени на спаун клонов
   // Image bigSpawnPanel;            //большая полоса на спаун
   // GameObject panelCont;

    public float spawnTime = 1;                    //значение полоски времени
    UIManager uiManager;
    public Transform streetTarget;                  //цель для людей на улице
    public Image capacityPanel;                            //полоса общего кол-ва людей
    Text totalCapacity;                             //текст общего кол-ва людей
    public GameObject noPeopleWarning;              //нету свободных меcn

    [Header("SPAWN K")]
    public float timePanelPerSecond;        //на сколько восстанавливается панель за тик
    public float minimuTimeRate;            //дно для плоски спауна
    public float minusTimePerSecond;        //на сколько отнимается полоска за секунду

    public bool isInfinity = false;
    public bool enaughVeh = true;

    public PeopleSpawner spawner;
    public LevelManager levelManager;

    public float cloneLayingRate;       //сколько клонов появится за секунды
    public float clonesPerSecond = 0;
    float addToNextSec = 0;
    public bool ready = false;

    //points
    Seeker seeker;
    Path path;
    public List<Path> pathPoints = new List<Path>();      //пути к хостелам
    public List<Transform> hostelTargets = new List<Transform>();
    public List<StarHostel> starHostels = new List<StarHostel>();
    int counter = 0;

    public GameObject hostelPanel;
    bool visible = false;

    private void Awake()
    {
        SharedInstance = this;

        foreach (Transform _tr in hostelTargets)
        {
            starHostels.Add(_tr.GetComponentInParent<StarHostel>());
        }
    }

    private void Start()
    {
        uiManager = UIManager.SharedInstance;
        timePanel = uiManager.timeSpawnPanel;
        capacityPanel = uiManager.capacityScale;
        totalCapacity = uiManager.totalCapacityText;
        CheckAvailPlaces();

        //panelCont = Camera.main.GetComponent<SimpleCamMove>().tapPanel;
        //bigSpawnPanel = Camera.main.GetComponent<SimpleCamMove>().fillPanel;
        UpdCapacityScale(); //(?)

        seeker = GetComponent<Seeker>();

        seeker.StartPath(spawnPlace.position, hostelTargets[counter].position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
        } else
        {
            print(p.error);
        }

        pathPoints.Add(path);
        counter++;

        if(counter < hostelTargets.Count)
            seeker.StartPath(spawnPlace.position, hostelTargets[counter].position, OnPathComplete);
    }

    private void FixedUpdate()
    {
        if (!ready)
            return;

        if (hostelPanel.transform.position.x < 600 && visible == false)//когда панель попадает в поле зрения камеры
        {
            visible = true;
            UpdCapacityScale();
        }
        else if (hostelPanel.transform.position.x > 600 && visible == true)
        {
            visible = false;
        }

        if (isInfinity == false)            //еслит не вкл буст на 30 сек
        {
            if (spawnTime < 1)
                spawnTime += timePanelPerSecond * Time.fixedDeltaTime;

            timePanel.fillAmount = spawnTime;
            timePanel.color = Color.Lerp(Color.red, Color.green, spawnTime);

            /*if (panelCont.activeSelf)
            {
                bigSpawnPanel.fillAmount = spawnTime;
            }*/
        } else          //вкл буст на бесконечность
        {
            spawnTime = 1;
            timePanel.fillAmount = spawnTime;
            timePanel.color = Color.green;


            /*if (panelCont.activeSelf)
            {
                bigSpawnPanel.fillAmount = spawnTime;
            }*/
        }

        uiManager.clonesOnScreenText.text = clonesSpawned - clonesNum + "";

        clonesPerSecond += clonesNum * cloneLayingRate + addToNextSec;
        if (clonesPerSecond >= 1 && enaughVeh)
        {
            SendNewStarsToHostel((int)clonesPerSecond);

            addToNextSec = clonesPerSecond - (int)clonesPerSecond;
            addToNextSec = Mathf.Abs(addToNextSec);

            clonesPerSecond = 0;
        }
    }

    public void SendNewStarsToHostel(int _num)          //расселение детей по хостелам и проверка свободных мест
    {
        if (clonesSpawned + _num <= availableSlotsHostel)       //ести ли места в хостелах для детей
        {
            for (int i = 0; i < _num; i++)      //расселяю кажого клона
            {
                foreach (GameObject _hostel in uiManager.allHostels)
                {
                    if (_hostel.GetComponent<StarHostel>().currentCapacity < _hostel.GetComponent<StarHostel>().hostelCapacity)
                    {
                        _hostel.GetComponent<StarHostel>().currentCapacity++;
                        _hostel.GetComponent<StarHostel>().UpdHostelScaleCapacity();
                        UpdateClonesNum();
                        clonesSpawned++;
                        //return;
                        break;
                    }
                }
            }
        } else//иеста хватает не для всех
        {
            if(_num > 1)
            {
                print("recalculate");
                _num = availableSlotsHostel - clonesSpawned;
                _num--;
                SendNewStarsToHostel(_num);
            }
        }
    }

    void UpdCapacityScale()         //апд полоски заполнения всех хостелов
    {
        if (!visible)
            return;
        float k = (clonesNum*1f) / (availableSlotsHostel*1f);
        capacityPanel.fillAmount = k;
        capacityPanel.color = Color.Lerp(Color.green, Color.red, k);
    }

    public void UpdAvailableSlots(int _count)           //апд доступных слотов
    {
        if(totalCapacity == null)
            totalCapacity = UIManager.SharedInstance.totalCapacityText;
        if(capacityPanel == null)
            capacityPanel = UIManager.SharedInstance.capacityScale;

        availableSlotsHostel += _count;
        totalCapacity.text = Economics.SharedInstance.getShortIndex(availableSlotsHostel);

        UpdCapacityScale();
    }

    public void SpawnClone()                //привязано к кнопке --- NO.
    {
        if (currentClone == null)
            return;

        GameObject _person = null;

        _person = ObjectPooler.SharedInstance.GetPooledObject("Clone");

        if (_person != null)
        {
            _person.transform.position = spawnPlace.transform.position;
            _person.transform.rotation = spawnPlace.transform.rotation;
            _person.SetActive(true);

            _person.GetComponent<ClonePath>().SetupClone();
            //_person.GetComponent<ClonePath>().enabled = true;
            //_person.GetComponent<ClonePath>().bodies[levelManager.currentLevel].SetActive(true);
        }
    }

    public void UpdateClonesNum()           //update ui
    {
        clonesNum++;
        UpdUIClonesNum();

        //if (Stats.SharedStats.visible)
        //    Stats.SharedStats.population.text = clonesNum + "";
    }

    public void UpdUIClonesNum()
    {
        clonesText.text = "" + Economics.SharedInstance.getShortIndex(clonesNum);
        UpdCapacityScale();
        //
        float k = (clonesNum * 1f) / (availableSlotsHostel * 1f);
        capacityPanel.fillAmount = k;
        capacityPanel.color = Color.Lerp(Color.green, Color.red, k);
        //
    }

    public void AttractPeople(float _num, string _tag, Transform _people)
    {
        CheckAvailPlaces();

        if (spawnTime < 0f)             //delay for spawn   
        {
            //print("to -1");
            spawnTime = minimuTimeRate;
            return;
        }

        if (enaughVeh == false)         //хватает ли вместимости машин
            return;

        if (spawnTime > 0)                //ДЛЯ ПРИВЛЕЧЕНИЯ ЛЮДЕЙ С УЛИЦЫ
        {
            for (int i = 0; i < _num; i++)
            {
                if (clonesSpawned >= availableSlotsHostel)
                    return;

                spawner.SpawnPerson(_tag, _people);
            }
        }
    }

    public void CheckAvailPlaces()
    {
        if (clonesSpawned >= availableSlotsHostel)          //проверка на кол-во свободных мест, спаун предупреждения
        {
            if (noPeopleWarning.activeSelf == false)
            {
                noPeopleWarning.SetActive(true);
                noPeopleWarning.GetComponent<PostBox>().SpawnNotif();
            }
            return;
        }
        else
        {
            if (noPeopleWarning.activeSelf)
            {
                noPeopleWarning.SetActive(false);
                noPeopleWarning.GetComponent<PostBox>().HideNotif();
            }
        }
    }
}
