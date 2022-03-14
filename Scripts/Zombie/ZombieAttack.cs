using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieAttack : MonoBehaviour {

	public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
    public int attackDamage = 10;               // The amount of health taken away per attack.


    Animator anim;                              // Reference to the animator component.
    GameObject player;                          // Reference to the player GameObject.
    PlayerVitals playerHealth;                  // Reference to the player's health.
    ZombieHealth zombieHealth;                    // Reference to this enemy's health.
    bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
    float timer;                                // Timer for counting up to the next attack.


    void Awake ()
    {
        // Setting up the references.
        //player = GameObject.FindGameObjectWithTag ("Player");
        //playerHealth = player.GetComponent <PlayerVitals> ();
        zombieHealth = GetComponent<ZombieHealth>();
        anim = GetComponent <Animator> ();
    }

	public void MyTriggerEnter(GameObject playerhit){
		 //If the entering collider is the player...
		//player = other.GetComponentInParent

		if(playerhit && playerhit.tag == "Player")
		{
			// ... the player is in range.
			player = playerhit;
			playerInRange = true;
		}
		//Debug.Log("My Trigger Enter");
	
	}
	public void MyTriggerExit(GameObject playerhit){
		// If the exiting collider is the player...
		if(playerhit && playerhit.tag == "Player")
		{
			// ... the player is no longer in range.
			//player = ;
			playerInRange = false;
		}
		//Debug.Log("My Trigger Enter");
	}




    void Update ()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
        if(timer >= timeBetweenAttacks && playerInRange && zombieHealth.currentHealth > 0)
        {
            // ... attack.
            Attack ();
        }

        // If the player has zero or less health...
        //if(playerHealth.hitPoints <= 0)
        //{
            // ... tell the animator the player is dead.
		//	anim.SetBool("PlayerDead",true);
        //}
    }


    public void Attack ()
    {
		anim.SetTrigger ("PlayerContact");
		//anim.SetFloat ("Attack",1f);
        // Reset the timer.
        timer = 0f;
		playerHealth = player.GetComponent <PlayerVitals> ();
        // If the player has health to lose...
        if(playerHealth.hitPoints > 0)
        {
            // ... damage the player.
           playerHealth.ApplyDamage (attackDamage,1);
        }
    }	

}
