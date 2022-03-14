using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour {

	public Transform player;
	ZombieHealth zombieHealth;
	ZombieValues zvalues;
	Animator anim;
	//EnemyHealth enemyHealth;
	NavMeshAgent nav;
	public float rushDistance = 3.0f;
	public float rushSpeedMultiplier = 3.0f ;
	float currentHP = 100;
	public float baseSpeed = 1;
	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		zombieHealth = gameObject.GetComponent<ZombieHealth> ();
		zvalues = gameObject.GetComponent<ZombieValues> ();
		nav = GetComponent<NavMeshAgent> ();
		nav.speed = baseSpeed;
		anim = GetComponent <Animator> ();
	}
	
	// Update is called once per frame
	void Update () {


		currentHP = zombieHealth.currentHealth;
		if (currentHP > 0) {
			if (Distance (player) < rushDistance) {
				//anim.SetTrigger ("PlayerContact");
				nav.speed = baseSpeed * rushSpeedMultiplier;
				anim.SetFloat ("WalkSpeed", nav.speed/2);
				//anim.speed = 0.8f;
			} 
			else {
				nav.speed = baseSpeed;
				anim.SetFloat ("WalkSpeed", nav.speed);
				//anim.speed = 1.0f;
			}
			nav.SetDestination (player.position);
			//Debug.Log (player.name);
			//Debug.Log (player.parent);
			//Debug.Log (player.position);
		}
		//else
		//	nav.Stop ();
	}
	float Distance(Transform player){
		Vector3 distance;
		distance = player.position - this.transform.position;

		return distance.magnitude;
	
	}

}
