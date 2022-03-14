using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Security.Policy;

public enum ZombieType{normal,prof,red}

public class SpawnSystem : MonoBehaviour {
	Transform player;
	SpawnManager sM;
	ZombieHealth zH;
	ZombieAttack zA;
	ZombieMovement zM;
	int zone;
	static int crCount = 0;
	bool canSpawn = true;
	public GameObject zombie;
	public GameObject zombieProf;
	public GameObject zombieRed;
	public Transform[] spawnPoints;
	public float spawnInterval = 3f;
	public int spawnLimit = 10;
	public int spawnCount = 0;
	public ZombieType spawnType;
	public bool spawnActive = false;
	public float distancePlayer;
	float nextSpawnTime = 0f;
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		spawnCount = 0;

	}
	public void SpawnZombiesAccToLevel(SpawnManager spawnManager,int zombieCount,ZombieType zombieType,int levelZone){
		spawnCount = 0;
		spawnLimit = zombieCount;
		spawnType = zombieType;
		spawnActive = true;
		sM = spawnManager;
		zone = levelZone;
		//StartSpawning ();
	
	}

	void Update () {
		if (spawnActive && canSpawn && spawnCount <= spawnLimit) {
			canSpawn = false;
			StartCoroutine (StartSpawning ());
			crCount++;
			//Debug.Log (crCount);
		}

		if (spawnCount >= spawnLimit)
			spawnActive = false;



		 
		distancePlayer = (gameObject.transform.position - player.position).magnitude;
	}
	IEnumerator StartSpawning(){
		canSpawn = false;
		//while(spawnActive&&spawnCount<=spawnLimit){
			yield return new WaitForSeconds(spawnInterval);
			SpawnZombie();
			spawnCount++;
		//}
		canSpawn = true;
		Debug.Log ("ZombieSpawned");
	}

	void SpawnZombie()
	{
		//Debug.Log(spawnPoints.Length);
		int random = Random.Range(0, spawnPoints.Length);
		//Debug.Log ("SpawnPointsLength / random");
		//Debug.Log (spawnPoints.Length);
		//Debug.Log (random);

		GameObject clone;
		GameObject templateZombie;
		if (spawnType == ZombieType.normal) {
			templateZombie = zombie;
		}
		else if (spawnType == ZombieType.prof) {
			templateZombie = zombieProf;
		} 
		else if (spawnType == ZombieType.red) {
			templateZombie = zombieRed;
		} 
		else {
			templateZombie = zombie;
		}
		clone = Instantiate(templateZombie, spawnPoints[random].position, spawnPoints[random].rotation);
		zH = clone.GetComponent<ZombieHealth> ();
		zA = clone.GetComponent<ZombieAttack> ();
		zM = clone.GetComponent<ZombieMovement> ();
		//float rs = Random.Range (0.8f,1.6f);
		//clone.GetComponent<NavMeshAgent> ().speed = sM.zombieBaseSpeed;
		zM.baseSpeed = zM.baseSpeed + (zone * 0.5f);
		zM.rushDistance = zM.rushDistance + (zone * 0.5f) ;

		zH.startingHealth = zH.startingHealth + (zone * 50);
		zH.currentHealth = zH.startingHealth;

		zA.attackDamage = zA.attackDamage + (zone * 5);
		zA.timeBetweenAttacks = zA.timeBetweenAttacks - (zone*0.02f);

		ZombieHealth.zombieAlive++;
		//spawnCount++;
	}


}
