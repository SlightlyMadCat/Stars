using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
	public float directionChangeInterval = 1;
	public float maxHeadingChange = 30;

	public bool run;
	public Animator Animator;
	NavMeshAgent controller;
	float heading;
	Vector3 targetPosition;
	Quaternion targetRotation;
 
	void Awake ()
	{
		controller = GetComponent<NavMeshAgent>();
 
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);
		run = Random.Range(0, 1.0f) > 0.8;
 
		StartCoroutine(NewHeading());
		if (run)
		{
			Animator.SetTrigger("Run");
			controller.speed = 14;
		}
	}
 
	void Update ()
	{
		if (controller.remainingDistance < 1.5)
		{
			controller.SetDestination(targetPosition);
			NewHeadingRoutine();
		}
	}
 
	IEnumerator NewHeading ()
	{
		while (true) {
			NewHeadingRoutine();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
 
	void NewHeadingRoutine ()
	{
		targetPosition = new Vector3(Random.Range(-98,98), 0, Random.Range(-98, 98));
	}
}