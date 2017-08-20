using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClonePath : MonoBehaviour {

    StarHostel starHostel;

    public Transform target;        //цель
    public float speed;     //скорость движения
    public float minDist;       //дистанция для переключения на след точку
    public float distanceToTarget;      //расстояние до цели
    public float rangeTimer;            //таймер на нахождение возле цели
    float currentTimer = 0;             //текущее значение таймера
    bool startTimer = false;

    double extraPrize;

    Seeker seeker;
    Path path;
    //CharacterController characterController;
    Economics economics;
    UIManager ui;
    CloneCenter cloneCenter;

    Vector3 dir;
   // Vector3 oldDir;
    int currentWaypoint;
    //int targetCounter = 0;
    public string oldTag;       //tag перед attract
    public GameObject[] bodies;     //тела клонов - для каждого уровня - свое

    public Vector3[] wayPoints;
    //Vector3 startPos;
    bool prefabSet = false;

    private void OnEnable()
    {
        //GetComponent<AlternativePath>().enabled = true;
        //seeker = GetComponent<Seeker>();
        //StartNewPath();
        // characterController = GetComponent<CharacterController>();

        //startPos = transform.position;
        /*if (!prefabSet)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                if (i != LevelManager.SharedInstance.currentLevel)
                {
                    Destroy(bodies[i]);
                } else
                {
                    //print("setBody");
                    bodies[i].SetActive(true);
                }
            }
            prefabSet = true;
            return;
        }*/
        /*SetupClone();*/
    }

    void SetModel()
    {
        if (!prefabSet)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                if (i != LevelManager.SharedInstance.currentLevel)
                {
                    Destroy(bodies[i]);
                }
                else
                {
                    //print("setBody");
                    bodies[i].SetActive(true);
                }
            }
            prefabSet = true;
            return;
        }
    }

    public void SetupClone()
    {
        SetModel();
        seeker = GetComponent<Seeker>();
        //print("11");
        StartNewPath();
        //characterController = GetComponent<CharacterController>();
    }

    public void OnPathComplete(Path p)      //закольцованное движение
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            wayPoints = path.vectorPath.ToArray();
        } else
        {
            print(p.error);
        }
    }

   /* private void Update()
    {
        if (wayPoints.Length == 0 || target == null)
            return;

        if (currentWaypoint >= wayPoints.Length)                   //добавить сюда анимацию idle
        {
            SetCloneToHostel();
            return;
        }

        Vector3 dirRight = new Vector3(-100, transform.position.y, transform.position.z);
        dir = (wayPoints[currentWaypoint] - transform.position);

        float angle = Vector3.Angle(dir, dirRight);
        if (dir.z > 0)
            angle = -angle;

        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 5);

        //Transform.Translate();
        transform.position += dir.normalized * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, wayPoints[currentWaypoint]) < minDist)
        {
            currentWaypoint++;
        }
    }*/

    public void Move()
    {
        if (wayPoints.Length == 0 || target == null)
            return;

        if (currentWaypoint >= wayPoints.Length)                   //добавить сюда анимацию idle
        {
            SetCloneToHostel();
            return;
        }

        Vector3 dirRight = new Vector3(-100, transform.position.y, transform.position.z);
        dir = (wayPoints[currentWaypoint] - transform.position);

        float angle = Vector3.Angle(dir, dirRight);
        if (dir.z > 0)
            angle = -angle;

        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 5);

        //Transform.Translate();
        transform.position += dir.normalized * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, wayPoints[currentWaypoint]) < minDist)
        {
            currentWaypoint++;
        }
    }

    void SetCloneToHostel()
    {
        if (starHostel.currentCapacity < starHostel.hostelCapacity)
        {
            cloneCenter.UpdateClonesNum();
            starHostel.UpdCapacity();

            startTimer = false;
            currentTimer = 0;
            currentWaypoint = 0;
            target = null;

            StarsCont.SharedInstance.stars.Remove(this);
            gameObject.SetActive(false);
        } else
        {
            StartNewPath();
        }
    }

    public void StartNewPath()
    {
        //if (cloneCenter == null)
            cloneCenter = CloneCenter.SharedInstance;

        float maxCapacity = 0;
        List<StarHostel> hostels = cloneCenter.starHostels;

        foreach (StarHostel _hostel in hostels)
        {
            if (_hostel.currentCapacity < _hostel.hostelCapacity)
            {
                if (_hostel.hostelCapacity - _hostel.currentCapacity > maxCapacity)
                {
                    maxCapacity = _hostel.hostelCapacity - _hostel.currentCapacity;
                    target = cloneCenter.hostelTargets[hostels.IndexOf(_hostel)];
                }
            }
        }

        starHostel = target.gameObject.GetComponentInParent<StarHostel>();

        //if (cloneCenter.hostelTargets.Contains(target) && Vector3.Distance(transform.position, startPos) < 3)
        //{
            path = cloneCenter.pathPoints[cloneCenter.hostelTargets.IndexOf(target)];
            wayPoints = new Vector3[path.vectorPath.Count];

            for (int i = 0; i < path.vectorPath.Count; i++)
            {
                if (i != 0 || i != path.vectorPath.Count)
                    wayPoints[i] = (path.vectorPath[i] + new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)) * 0.1f);
                else
                    wayPoints[i] = (path.vectorPath[i]);
            }
        //}
        /*else
        {
            //print("new path");
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }*/
        currentWaypoint = 0;
        StarsCont.SharedInstance.stars.Add(this);
    }

    private void OnMouseDown()      //click on extra pers
    {
        //print("click");
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ui = UIManager.SharedInstance;
        ui.ShowExtraPersScreen(1);
        ui.uiAnimators[14].GetComponentInChildren<Button>().onClick.AddListener(() => GetBonusFromExtraPers());
        CalcExtraPrize();
        ui.uiAnimators[14].GetComponentInChildren<Text>().text = economics.getShortIndex(extraPrize)+" $";
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
    }
}
