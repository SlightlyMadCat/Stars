using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PeopleSpawner : MonoBehaviour {
    public GameObject[] people;     //массив всех префабов для спауна
    public float Cd;            //кд на спаун
    float randomCd = 0;                 //рандомное время на спаун
    float currentCd;            //текущее кд
    //public Vector3 moveDir;      //направление движения человка
    public Transform personTarget;  //цель для движения по улице
    public CloneCenter cloneCenter;

    public GameObject crowdPart;
    public List<GameObject> spawnedCrowds;

    public GameObject message;

    private void Start()
    {
        currentCd = Cd;
        StartCoroutine(SpawnMessage());
        //StartCoroutine(WaitAndPrint());
    }

    private void FixedUpdate()
    {
        if (currentCd < randomCd)
        {
            currentCd += Time.fixedDeltaTime * 10;
        } else
        {
            //SpawnPerson();
            SpawnCrowd();
            currentCd = 0;
            randomCd = Random.Range(Cd/5,Cd);
        }
    }

    //private int timer = 0;

    /*IEnumerator WaitAndPrint()
    {
        while (true)
        {
            SpawnCrowd();
            yield return new WaitForSeconds(Random.Range(Cd / 5, Cd));

            yield return null;
        }
    }*/

    public void StartCor()
    {
        StartCoroutine(SpawnMessage());
    }

    public IEnumerator SpawnMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);     //TODO _ SET TIME
            SpawnSpecMessage();
            yield break;
        }
    }

    public void SpawnSpecMessage()
    {
        if (spawnedCrowds.Count == 0)
            return;

        List<GameObject> availableCorwds = new List<GameObject>();

        for (int i = 0; i < spawnedCrowds.Count; i++)
        {
            if (spawnedCrowds[i].GetComponent<CrowdPart>().childCount > 0)        //если в этой толпе еще есть люди
            {
                availableCorwds.Add(spawnedCrowds[i]);
            }
        }

        if (message.GetComponent<CrowdTouch>().crowd != null)
        {
            availableCorwds.Remove(message.GetComponent<CrowdTouch>().crowd);
        }

        if (availableCorwds.Count == 0)
        {
            SpawnSpecMessage();
            return;
        }

        int j = Random.Range(0,availableCorwds.Count);
        GameObject _crowd = availableCorwds[j];
        j = Random.Range(0, _crowd.GetComponent<CrowdPart>().childCount);
        Transform _person = _crowd.transform.GetChild(j);

        message.SetActive(true);
        message.transform.position = _person.position;
        message.transform.SetParent(_person);
        message.transform.position += Vector3.up;

        CrowdTouch _touch = message.GetComponent<CrowdTouch>();
        _touch.parent = _person.gameObject;
        _touch.crowd = _crowd;
        _touch.spawner = this;
        _crowd.GetComponent<CrowdPart>().message = _touch;

        //print(message.transform.parent);
    }

    public void SpawnCrowd()
    {
        GameObject _crowd = ObjectPooler.SharedInstance.GetPooledObject("Crowd_0");

        _crowd.transform.position = transform.position;
        _crowd.transform.rotation = transform.rotation;
        _crowd.SetActive(true);

        _crowd.GetComponent<CrowdPart>().target = personTarget;
        CloneCenter.SharedInstance.spawner.spawnedCrowds.Add(_crowd);//в список толп на улице
    }

    public void SpawnPerson(/*Vector3 _touchPos, GameObject _crowd,*/ string _tag, Transform _people)          //спаун человека на улице
    {/*
        if (spawnedCrowds.Count == 0)
            return;

        GameObject _person = null;

        //List<GameObject> _crowdParts = new List<GameObject>();
        List<float> _distances = new List<float>();

        //_crowdParts = GameObject.FindGameObjectsWithTag("Crowd_0").ToList();
        List<GameObject> availableCorwds = new List<GameObject>();

        for(int i = 0; i < spawnedCrowds.Count; i++)
        {
            if (spawnedCrowds[i].GetComponent<CrowdPart>().childCount > 0)        //если в этой толпе еще есть люди
            {   
                _distances.Add(Vector3.Distance(spawnedCrowds[i].transform.position, _touchPos));
                availableCorwds.Add(spawnedCrowds[i]);
            }   
        }

        if (availableCorwds.Count == 0)
        {
            SpawnCrowd();
            availableCorwds.Add(spawnedCrowds.Last());
        }

        if (_distances.Count > 0)
        {
            float _minDist = _distances.Min();
            int index = _distances.IndexOf(_minDist);
            int _num = availableCorwds[index].transform.childCount - availableCorwds[index].GetComponent<CrowdPart>().childCount;*/

            GameObject _person;
            _person = ObjectPooler.SharedInstance.GetPooledObject(_tag); 

             _person.transform.position = _people.transform.position;
            _person.transform.rotation = _people.transform.rotation;

            _people.gameObject.SetActive(false);
            _people.GetComponentInParent<CrowdPart>().childCount--;

            _person.SetActive(true);
            //_person.gameObject.tag = "Person";
            //_person.GetComponent<CharacterController>().enabled = true;
            _person.GetComponent<Person>().enabled = true;
            _person.GetComponent<Person>().StartNewPath(cloneCenter.streetTarget);
            //_person.GetComponent<ClonePath>().enabled = false;

            cloneCenter.clonesSpawned++;
            cloneCenter.spawnTime -= cloneCenter.minusTimePerSecond;
        //}

        //cloneCenter.clonesSpawned++;
    }
}
