using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SpawnManager : NetworkBehaviour {

	Transform player;
	//public SpawnSystem[] spawnAreas;
	SpawnSystem[] spawnAreas;
	int gameLevel;
	int zone = 0;
	public int	totalZombies;
	public int	zombieHP;
	public float  zombieAttackInterval;
	public float  zombieRushDistance;
	public float  zombieBaseSpeed;
	public float  zombieRushSpeedMultiplier;
	public float  zombieSpawnInterval;
	public int zombieAttackDamage;
	public LevelManager levelM;

	public int total_normal,remaining_normal;
	public int total_prof,remaining_prof;
	public int total_red,remaining_red;

	public bool gameOn = true;
	public bool gameCompleted = false;
	float nextSpawnTime;
	public float levelDuration = 25f;
	void Start () {
		spawnAreas = gameObject.GetComponentsInChildren<SpawnSystem> ();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	public void SetLevel(int glevel){
		
		/*
		zombieHP = 100 + (zone * 50);
		zombieAttackInterval = 0.5f - (zone*0.02f);
		zombieRushSpeedMultiplier = 2f + (zone * 0.1f);
		zombieBaseSpeed = 1f;//+ level * 0.05f;
		zombieRushDistance = 3.0f + (zone * 0.2f);
		zombieSpawnInterval = 3f;
		zombieAttackDamage = 10 + zone;

	*/
		int level;
		if (zone == 5) {
			level = glevel;
			
		} else {
			level = (glevel - 1) % 15 + 1;
		}
		zone = glevel / 15;
		if (level <= 5) {
			total_normal = 5 * level;
			total_prof = 0;
			total_red = 0;
		
		} else if (level <= 10) {
			total_normal = (level - 6) * 5 / 2;
			total_prof = 5 + ((level - 6) * 5 / 2);
			total_red = 0;

		} else if (level <= 15) {
			total_normal = 0;
			total_prof = (level - 11) * 5 / 2;
			total_red = 5 + ((level - 11) * 5 / 2);
		
		
		} else {
			gameCompleted = true;
		}
		//total_normal = Random.Range (0,);
		gameLevel = level;
		totalZombies = total_red + total_prof + total_normal; 
		zombieSpawnInterval = levelDuration / totalZombies;
	
	}
	public IEnumerator SpawnZombies(){
		Debug.Log ("Spawn++");
		int zcount = 0;
		int[] spawn;
		spawn = new int[2];

		int randomSpawn;
		int randomType;
		int totalRemaining;
		int zombiePerSpawn = 1;

		remaining_normal = total_normal;
		remaining_prof = total_prof;
		remaining_red = total_red;
		totalRemaining = remaining_normal + remaining_prof + remaining_red;
		while (totalRemaining>0) {

			yield return new WaitForSeconds (zombieSpawnInterval);
				
			spawn [0] = GetSpawn (-1);
			spawn [1] = GetSpawn (spawn [0]);
			randomSpawn = Random.Range (0, 2);

			randomType = Random.Range (1, totalRemaining);

			if (randomType <= remaining_normal) {
				spawnAreas [spawn [randomSpawn]].SpawnZombiesAccToLevel (this, zombiePerSpawn, ZombieType.normal,zone);
				remaining_normal--;
			
			} else if (randomType <= remaining_normal + remaining_prof) {
				spawnAreas [spawn [randomSpawn]].SpawnZombiesAccToLevel (this, zombiePerSpawn, ZombieType.prof,zone);
				remaining_prof--;
			
			} else if (randomType <= totalRemaining) {
				spawnAreas [spawn [randomSpawn]].SpawnZombiesAccToLevel (this, zombiePerSpawn, ZombieType.red,zone);
				remaining_red--;
			
			}
			totalRemaining = remaining_normal + remaining_prof + remaining_red;
			//zcount--;


		}

		Debug.Log ("AllZombieSpawned");
		levelM.levelSpawnComplete = true;
	} 
	int GetSpawn(int exclude){
		int size = spawnAreas.Length;
		int min=0;


		if (exclude == 0)
			min = 1;
		int i;

		for(i=0;i<size;i++) {
			if(i!=exclude)
				if (spawnAreas[i].distancePlayer < spawnAreas[min].distancePlayer) {
					min = i;

				}
		
		}
		return min;
	}

}
