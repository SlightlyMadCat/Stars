using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrigger : MonoBehaviour {

    public bool Eggs;       //это триггер для яйцевоза
    public bool Post;           //триггер для почты
            
    public float carWaitTimer;      //время на ожидание
    public float curTimer = 0;

    public GameObject carInTrigger;
    float carSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Car>() == null)
            return;

        if (Post)
        {
            if (other.gameObject.GetComponent<Car>().post != null)
            {
                other.gameObject.GetComponent<Car>().SpawnPost();
            }
        }
            
        if (Eggs)           //когда машина зашла в область - на время остановить
        {
            if (other.gameObject.GetComponent<Car>().post == null)
            {
                carSpeed = other.gameObject.GetComponent<Car>().speed;
                other.gameObject.GetComponent<Car>().speed = 0;
                carInTrigger = other.gameObject;
            }
        }
    }

    private void FixedUpdate()
    {
        if (curTimer < carWaitTimer && carInTrigger != null)
        {
            curTimer += Time.fixedDeltaTime * 10;
        }
        else if (curTimer >= carWaitTimer && carInTrigger != null)
        {
            carInTrigger.GetComponent<Car>().speed = carSpeed;
            curTimer = 0;
            carInTrigger = null;
        }
    }
}
