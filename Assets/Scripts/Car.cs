using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {
    public Transform target;         //напрвелние движения человека
    public float speed;                 //скорость движения человека
    public GameObject post;         //префаб ящика
    Vector3 dir;

    [Header("TTX")]
    public string name;
    public double price;
    public double capacity;
    public Sprite icon;

    public Transform[] waypoints;
    int pointIndex = 0;
    public bool move = false;

    private void OnEnable()
    {
        //if(target != null)
        //    dir = target.position - transform.position;
        pointIndex = 0;
    }

    public void StartMove()
    {
        pointIndex = 0;
        move = true;
        //dir = waypoints[pointIndex].transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        if (!move)
            return;

        if (Vector3.Distance(waypoints[pointIndex].position, transform.position) < 2.5f)
        {
            if (pointIndex < waypoints.Length - 1)
                pointIndex++;
            else
            {
                gameObject.SetActive(false);
                move = false;
            }
        }

        Vector3 dir = waypoints[pointIndex].position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 3).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);

        //Debug.DrawLine(transform.position, waypoints[pointIndex].position);
        transform.position = Vector3.MoveTowards(transform.position, waypoints[pointIndex].position, speed);
    }

    public void SpawnPost()
    {
        GameObject _post = Instantiate(post, transform.position, post.transform.rotation);
        _post.GetComponent<Rigidbody>().AddForce((Vector3.forward*150)+(Vector3.up*150));
        Physics.IgnoreCollision(_post.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
