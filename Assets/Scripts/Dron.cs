using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dron : MonoBehaviour {
    public Transform spawnPos;      //позиция где дрон появился
    public Transform target;            //куда он летит
    Vector3 moveDirection;          //направление движения
    //float distToTarget = 0;     //дистанция до цели
    bool move = true;       //двигается ли дрое
    public bool targetBottom = false;       //цели внизу
    public bool targetTop = false;          //цель вверху
    public float speed;

    public float minPercent;
    public float maxPercent;

    public DronSpawner spawner;
    //public string money;         //бабки за сбитие дрона  - сделать рандом

    public GameObject moneyGetText;               //префаб текста про получение денег
    Economics economics;

    private void Start()
    {
        economics = Economics.SharedInstance;
        if (target == null)
            return;

        //Debug.DrawLine(spawnPos.position, target.position, Color.red, 15);
        moveDirection = target.position - spawnPos.position;

        Vector3 dirRight = new Vector3(-999, transform.position.y, transform.position.z);
        float angle = Vector3.Angle(moveDirection, dirRight);

        if (moveDirection.z > 0)
        {
            angle *= -1;
        }

        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, -angle+90, transform.eulerAngles.z);
        transform.rotation = newRot;

        //ДРОНЫ НЕ СТАЛКИВАЮТСЯ МЕЖДУ СОБОЙ И UI
        Physics.IgnoreLayerCollision(9,9,true);
        Physics.IgnoreLayerCollision(5,9, true);
    }

    private void Update()
    {
        if (move)
        {
            transform.position += moveDirection.normalized * Time.deltaTime * speed;
        }

        if (Vector3.Distance(transform.position, target.position) < 1)
            StopMove();
    }

    void StopMove()
    {
        move = false;

        if (targetBottom)
        {
            spawner.availableTop.Add(spawnPos);
            spawner.availableBottom.Add(target);
        }

        if (targetTop)
        {
            spawner.availableBottom.Add(spawnPos);
            spawner.availableTop.Add(target);
        }

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        //move = false;             //отключил чтобы дрон падал с горизонтальной скоростью
        GetComponentInChildren<Animator>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (move)
        {
            CalculateMoney();
            StopMove();
        }
    }

    void CalculateMoney()           //расчет дропа денег
    {
        float i = Random.Range(minPercent,maxPercent);         //вместо 10 - добавить прокачку
        double curPrize = economics.moneyPerSecond * i;      //получаем процент от FV

        if (economics.moneyPerSecond == 0)
            curPrize = Random.Range(minPercent,maxPercent) * 100;

        GameObject text = Instantiate(moneyGetText, transform.position, moneyGetText.transform.rotation);
        //text.text = economics.getShortIndex(curPrize) + " $";
        text.GetComponentInChildren<TextMeshPro>().text = economics.getShortIndex(curPrize) + " $";

        //print(economics.getShortIndex(curPrize));

        Destroy(text.gameObject, 0.5f);

        economics.ImmidUpd(curPrize);

        SaveManager.SharedInstance.dronesTakeDown++;       //сохран

        if (Stats.SharedStats.visible)
            Stats.SharedStats.droneTakeDowns.text = SaveManager.SharedInstance.dronesTakeDown + "";
    }
}
