using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {
    //public GameObject[] cars;     //массив всех префабов для спауна
    public float minCd;            //кд на спаун
    public float maxCd;
    float randomCd = 0;                 //рандомное время на спаун
    //float currentCd;            //текущее кд
    public Transform target;      //цель дуижения машины
    public string carTag;

    public BusScreenCont busScreen;
    public BusStation busStation;
    public List<GameObject> carList;

    public Transform wayCont;
    Transform[] waypoints;

    public bool isTrackSpawn;
    List<int> carPrefIndexes = new List<int>();     //списко номеров префабов машин, по ним можно ставить здания

    public HelSpawner helSpawner;
    public PlaneSpawner planeSpawner;

    public bool canSpawn = true;
    bool working = false;

    /*private void Start()
    {
        //randomCd = Random.Range(Cd / 2, Cd);
        currentCd = Cd;
    }*/

    public void SetCarsToSpawnList()           //добавляю префабы в спсиок машин на спаун + определяю какие здания должны быть активны
    {
        carList.Clear();
        helSpawner.helicopters.Clear();
        planeSpawner.plainPrefs.Clear();

        carPrefIndexes.Clear();

        foreach (int _i in busScreen.carIndexes)        //СОГЛАСОВАТЬ НОМЕРА ПРЕФАБОВ-машина
        {
            if (_i >= 0 && _i <3)
            {
                carList.Add(busStation.carPrefabs[_i]);
            } else if (_i == 3 || _i == 6)//-вертолет - дирижабль
            {
                helSpawner.helicopters.Add(busStation.carPrefabs[_i]);
            } else if (_i == 4 || _i == 5)//-самолет
            {
                planeSpawner.plainPrefs.Add(busStation.carPrefabs[_i]);
            }

            carPrefIndexes.Add(_i);
        }

        ChoseActiveBuildings();

        if (!working)
        {
            StartCoroutine(WaitAndPrint());
        }
    }

    void ChoseActiveBuildings()     //ккаие здания включить
    {

        /*if (carPrefIndexes.Contains(0) || carPrefIndexes.Contains(1) || carPrefIndexes.Contains(2))            // если куплен хоть один из автобусов
            busStation.stationPrefabs[0].SetActive(true);
        else
            busStation.stationPrefabs[0].SetActive(false);*/

        if (carPrefIndexes.Contains(3) || carPrefIndexes.Contains(6))            // если куплен вертолет или дирижабль
            busStation.stationPrefabs[1].SetActive(true);
        else
            busStation.stationPrefabs[1].SetActive(false);

        if (carPrefIndexes.Contains(4) || carPrefIndexes.Contains(5))            // если куплен самолет
            busStation.stationPrefabs[2].SetActive(true);
        else
            busStation.stationPrefabs[2].SetActive(false);
    }

    /*private void FixedUpdate()
    {
        if (currentCd < randomCd)
        {
            currentCd += Time.fixedDeltaTime * 10;
        }
        else
        {
            if(carList.Count > 0)
                SpawnCar();
            currentCd = 0;

            if(!isTrackSpawn)
                randomCd = Random.Range(Cd / 2, Cd);
            else 
                randomCd = Cd / carList.Count;
        }
    }*/

    //private int timer = 0;

    IEnumerator WaitAndPrint()
    {
        while (true)
        {
            working = true;
            if (carList.Count > 0)
                SpawnCar();
            //currentCd = 0;

            if (!isTrackSpawn)
                randomCd = Random.Range(minCd, maxCd);
            else
                randomCd = maxCd / carList.Count;

            yield return new WaitForSeconds(randomCd);

            yield return null;
        }
    }

    void Start()
    {
        if(!isTrackSpawn)
            StartCoroutine(WaitAndPrint());
    }

    void SpawnCar()          //спаун caar на улице
    {
        if (!isTrackSpawn)
        {
            if (!canSpawn)
                return;
        }

        GameObject _car;
        if (isTrackSpawn)
        {
            _car = Instantiate(carList[0], transform.position, carList[0].transform.rotation);
            carList.Add(carList[0]);
            carList.RemoveAt(0);
        }
        else
        {
            _car = Instantiate(carList[0], transform.position, carList[0].transform.rotation);
            //_car.transform.position = transform.position;
            // _car.transform.rotation = _car.transform.rotation;
            // _car.SetActive(true);
        }

        waypoints = new Transform[wayCont.transform.childCount];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = wayCont.transform.GetChild(i);
        }

        _car.GetComponent<Car>().waypoints = waypoints;
        _car.GetComponent<Car>().StartMove();

        //перенсти машину в конец списка на спауне
    }
}
