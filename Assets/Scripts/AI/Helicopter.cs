using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour {
    public Transform target;
    HelSpawner parent;

    bool startMove = false;
    bool moveBottom = false;

    public void Setup(Transform _target, HelSpawner _parent)
    {
        target = _target;
        transform.position = new Vector3(target.position.x, target.position.y + 25, target.position.z);
        parent = _parent;
        startMove = true;
        moveBottom = true;

        //print(target.transform.position);
    }

    private void Update()
    {
        if (!startMove)
            return;

        if (target.position.y < transform.position.y && moveBottom)
        {
            transform.Translate(Vector3.up * -Time.deltaTime * 3);
            //print("fff");
            return;
        }
        else if (target.position.y + 25 > transform.position.y && !moveBottom)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 3);
            return;
        }

        if (target.position.y >= transform.position.y)
        {
            moveBottom = false;
            return;
        }

        startMove = false;
        //parent.SetTarget();
        parent.helTargets.Add(target.gameObject);
        gameObject.SetActive(false);
    }
}
