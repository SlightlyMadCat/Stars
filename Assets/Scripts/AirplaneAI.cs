using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneAI : MonoBehaviour {

    public Transform[] waypoints;
    int wayIndex = 0;
    public float speed;
    public float rotSpeed;

    private void Start()
    {
        transform.position = waypoints[wayIndex].position;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, waypoints[wayIndex].position) > 1f)
        {
            //transform.LookAt(waypoints[wayIndex].position);
            Vector3 dir = waypoints[wayIndex].position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);

        } else
        {
            if (wayIndex < waypoints.Length - 1)
                wayIndex++;
            else
            {
                PlaneSpawner.SharedInstance.SpawnPlane();
                gameObject.SetActive(false);            //выкл самолет после прохождения дист
                return;
            }
        }

        //transform.rotation = Quaternion.Lerp(transform.rotation, hyroScop.rotation, 0.05f);

        //Debug.DrawLine(transform.position, waypoints[wayIndex].position);
        //transform.Translate(transform.forward * 0.1f);
        transform.position = Vector3.MoveTowards(transform.position, waypoints[wayIndex].position, speed);
    }
}
