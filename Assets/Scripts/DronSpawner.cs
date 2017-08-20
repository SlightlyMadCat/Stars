using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronSpawner : MonoBehaviour {
    public List<Transform> pointsTop = new List<Transform>();           //список всех точек спауна сверху и синзу
    public List<Transform> pointsBottom = new List<Transform>();

    public List<Transform> availableTop = new List<Transform>();        //списки доступных точек сверху и снизу
    public List<Transform> availableBottom = new List<Transform>();

    public GameObject dron;             //префаб дрона

    public float spawnCd;               //кд на спаун
    //float curCd = 0;            //текущее кд

    private void Start()
    {
        availableTop = pointsTop;
        availableBottom = pointsBottom;

        StartCoroutine(WaitAndPrint());
    }

    /*private void FixedUpdate()
    {
        if (curCd < spawnCd)
            curCd += Time.fixedDeltaTime * 10;
        else
            Spawn();
    }*/

    IEnumerator WaitAndPrint()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnCd);

            yield return null;
        }
    }

    void Spawn()
    {
        int top = Random.Range(0, availableTop.Count);
        int bottom = Random.Range(0, availableBottom.Count);

        //GameObject _dron = ObjectPooler.SharedInstance.GetPooledObject("Dron");
        //_dron.transform.rotation = dron.transform.rotation;

        //Dron _dr = _dron.GetComponent<Dron>();

        if (top % 2 == 0)
        {
            GameObject _dron = Instantiate(dron, availableTop[top].position, dron.transform.rotation);
            //_dron.transform.position = availableTop[top].transform.position;

            Dron _dr = _dron.GetComponent<Dron>();
            _dr.target = availableBottom[bottom];
            _dr.spawnPos = availableTop[top];
            _dr.spawner = this;
            _dr.targetBottom = true;

        } else
        {
            GameObject _dron = Instantiate(dron, availableBottom[bottom].position, dron.transform.rotation);
            _dron.transform.position = availableBottom[bottom].position;

            Dron _dr = _dron.GetComponent<Dron>();
            _dr.target = availableTop[top];
            _dr.spawnPos = availableBottom[bottom];
            _dr.spawner = this;
            _dr.targetTop = true;
        }

        //_dron.SetActive(true);

        availableTop.Remove(availableTop[top]);
        availableBottom.Remove(availableBottom[bottom]);

        //curCd = 0;
    }
}
