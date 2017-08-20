using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Person : MonoBehaviour
{
    public Transform target;        //цель
    public float speed;     //скорость движения
    public float minDist;       //дистанция для переключения на след точку
    public float distanceToTarget;      //расстояние до цели
    public float rangeTimer;            //таймер на нахождение возле цели
    //float currentTimer = 0;             //текущее значение таймера
    //bool startTimer = false;

    double extraPrize;

    Seeker seeker;
    Path path;
    Economics economics;
    UIManager ui;
    //CharacterController characterController;

    Vector3 dir;
    // Vector3 oldDir;
    int currentWaypoint;
    //int targetCounter = 0;

    Vector3[] wayPoints;

    private void Start()
    {
        //characterController = GetComponent<CharacterController>();
    }

    public void OnPathComplete(Path p)      //закольцованное движение
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;

            wayPoints = new Vector3[path.vectorPath.Count];
            for (int i = 0; i < path.vectorPath.Count; i++)
            {
                if (i != 0 || i != path.vectorPath.Count - 1)
                    wayPoints[i] = (path.vectorPath[i] + new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)) * 0.1f);
                else if (i == path.vectorPath.Count)     //если это последняя точка
                    wayPoints[i] = (path.vectorPath[i]);
                else if (i == 0)        //если это первая точка
                    wayPoints[i] = transform.position;
            }

            StarsCont.SharedInstance.people.Add(this);
        }
        else
        {
            print(p.error);
        }
    }

    /*private void Update()
    {
        if (path == null || target == null)
            return;

        if (currentWaypoint >= wayPoints.Length)
        {
            //StartNewPath();
            //if (gameObject.tag != "Clone")
                CloneCenter.SharedInstance.SpawnClone();

            gameObject.SetActive(false);
            //Destroy(gameObject);
            return;
        }

        Vector3 dirRight = new Vector3(-100, transform.position.y, transform.position.z);
        dir = (wayPoints[currentWaypoint] - transform.position);

        float angle = Vector3.Angle(dir, dirRight);

        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 5);

        //Debug.DrawLine(transform.position, dirRight);

        transform.position += dir.normalized * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, wayPoints[currentWaypoint]) < minDist)
        {
            currentWaypoint++;
        }
    }*/

    public void Move()
    {
        if (path == null || target == null)
            return;

        if (currentWaypoint >= wayPoints.Length)
        {
            //StartNewPath();
            //if (gameObject.tag != "Clone")
            CloneCenter.SharedInstance.SpawnClone();
            StarsCont.SharedInstance.people.Remove(this);

            gameObject.SetActive(false);
            //Destroy(gameObject);
            return;
        }

        Vector3 dirRight = new Vector3(-100, transform.position.y, transform.position.z);
        dir = (wayPoints[currentWaypoint] - transform.position);

        float angle = Vector3.Angle(dir, dirRight);

        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 5);

        //Debug.DrawLine(transform.position, dirRight);

        transform.position += dir.normalized * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, wayPoints[currentWaypoint]) < minDist)
        {
            currentWaypoint++;
        }
    }

    public void StartNewPath(Transform _target)
    {
        //GetComponent<ClonePath>().enabled = false;
        seeker = GetComponent<Seeker>();
        target = _target;
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        currentWaypoint = 0;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

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
    }
}
