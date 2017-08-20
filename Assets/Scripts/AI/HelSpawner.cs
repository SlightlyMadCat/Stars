using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HelSpawner : MonoBehaviour {

    public float totalCd;
    float curCd = 0;
    float randCd = 0;

    public List<GameObject> helicopters = new List<GameObject>();
    public List<GameObject> helTargets;

    private void Awake()
    {
        helTargets = GameObject.FindGameObjectsWithTag("HelicopterPlace").ToList();     //возможно добавить сюда другие здания с Н
    }

    /*private void Start()
    {
        SetTarget();
    }

    public void SetTarget()            //спауню вертолет и даю ему цель
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("HelicopterPlace");
        int i = Random.Range(0, targets.Length);
        //print(i);

        GameObject _hel = ObjectPooler.SharedInstance.GetPooledObject("Helicopter");
        _hel.SetActive(true);
        _hel.GetComponent<Helicopter>().Setup(targets[i].transform, this);
    }*/

    private void FixedUpdate()
    {
        if(helicopters.Count > 0)
        {
            if(curCd >= randCd)
            {
                if (helicopters.Count > 0 && helTargets.Count > 0)
                {
                    SpawnHelic();
                }

                curCd = 0;
                randCd = totalCd / helicopters.Count;
            } else
            {
                curCd += Time.fixedDeltaTime * 10;
            }
        }
    }

    void SpawnHelic()
    {
        int i = Random.Range(0, helTargets.Count);
        //print(i);

        //print(helTargets.Count);
        GameObject _hel = Instantiate(helicopters[0]);
        //_hel.SetActive(true);
        _hel.GetComponent<Helicopter>().Setup(helTargets[i].transform, this);

        helTargets.Remove(_hel.GetComponent<Helicopter>().target.gameObject);

        helicopters.Add(helicopters[0]);
        helicopters.RemoveAt(0);
    }
}
