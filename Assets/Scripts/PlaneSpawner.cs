using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour {

    public static PlaneSpawner SharedInstance;

    public List<GameObject> plainPrefs = new List<GameObject>();
    Transform[] waypoints;

    public float Cd;
    float curCd = 0;
    float randCd = 0;

    private void Awake()
    {
        SharedInstance = this;

        waypoints = new Transform[transform.childCount];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }

    private void Start()
    {
        SpawnPlane();
    }

    /*private void FixedUpdate()
    {
        if (curCd >= randCd)
        {
            if (plainPrefs.Count > 0)
            {
                SpawnPlane();
            }
            curCd = 0;

            randCd = Cd / plainPrefs.Count;
        } else
        {
            curCd += Time.fixedDeltaTime * 10;
        }
    }*/

    public void SpawnPlane()       //spawn aircraft
    {
        GameObject _plane = Instantiate(plainPrefs[0], waypoints[0].transform.position, plainPrefs[0].transform.rotation);
        _plane.GetComponent<AirplaneAI>().waypoints = waypoints;

        plainPrefs.Add(plainPrefs[0]);
        plainPrefs.RemoveAt(0);
    }
}
