using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSG.MeshAnimator;

public class CrowdPart : MonoBehaviour {
    public Transform target;
    public int childCount;
    public float speed = 1;

    public CrowdTouch message;

    private void OnEnable()         //randomize transform position
    {
        //CloneCenter.SharedInstance.spawner.spawnedCrowds.Add(gameObject);//в список толп на улице
        childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).transform.position = transform.TransformPoint(new Vector3(Random.Range(0,10), 0, Random.Range(0, 10)) * 0.2f);
            transform.GetChild(i).gameObject.SetActive(true);

            if (transform.GetChild(i).GetComponent<MeshAnimator>())         //изменяю скорость чтобы толпа была более не однородной
            {
                MeshAnimator ma = transform.GetChild(i).GetComponent<MeshAnimator>();
                //ma.defaultAnimation = ma.animations[Random.Range(0, ma.animations.Length)];
                ma.speed = Random.Range(0.9f, 1.1f);
            } else if (transform.GetChild(i).GetComponentInChildren<MeshAnimator>())
            {
                MeshAnimator ma = transform.GetChild(i).GetComponentInChildren<MeshAnimator>();
                //ma.defaultAnimation = ma.animations[Random.Range(0, ma.animations.Length)];
                ma.speed = Random.Range(0.9f, 1.1f);
            }
        }

        StarsCont.SharedInstance.crowds.Add(this);
    }

    /*private void Update()
    {
        if(target != null)
        {
            //Vector3 dir = target.position - transform.position;
            //transform.position += dir.normalized * Time.fixedDeltaTime;
            transform.Translate(Vector3.right * -Time.deltaTime *1.5f);

            if(Vector3.Distance(transform.position, target.position) < 2)
            {
                if (message)
                {
                    //print("sbros");
                    message.transform.SetParent(null);
                    message.gameObject.SetActive(false);
                    message = null;
                }
            }

            if(Vector3.Distance(transform.position, target.position) < 1)
            {
                target = null;
                CloneCenter.SharedInstance.spawner.spawnedCrowds.Remove(gameObject);//в список толп на улице

                gameObject.SetActive(false);
                return;
            }
        }
    }*/

    public void Move()
    {
        if (target != null)
        {
            //Vector3 dir = target.position - transform.position;
            //transform.position += dir.normalized * Time.fixedDeltaTime;
            transform.Translate(Vector3.right * -Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, target.position) < 2)
            {
                if (message)
                {
                    //print("sbros");
                    message.transform.SetParent(null);
                    message.gameObject.SetActive(false);
                    message = null;
                }
            }

            if (Vector3.Distance(transform.position, target.position) < 1)
            {
                target = null;
                CloneCenter.SharedInstance.spawner.spawnedCrowds.Remove(gameObject);//в список толп на улице

                StarsCont.SharedInstance.crowds.Remove(this);
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
