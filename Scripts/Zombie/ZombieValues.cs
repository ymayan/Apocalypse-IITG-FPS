using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieValues : MonoBehaviour {

	public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
	public int attackDamage = 10;               // The amount of health taken away per attack.
	public int startingHealth = 100;            // The amount of health the enemy starts the game with.
	public int currentHealth;                   // The current health the enemy has.
	public float sinkSpeed = 0.5f;              // The speed at which the enemy sinks through the floor when dead.
	public int scoreValue = 10; 
	public float baseSpeed = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
