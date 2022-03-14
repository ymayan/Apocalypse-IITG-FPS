using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	public Text levelText;
	public int gameLevel = 1;
	public int difficulty = 1;
	public bool levelCompleted = false;
	public bool levelSpawnComplete = false;
	public bool levelActive = false;
	public SpawnManager spawnM;
	public FogScript fogParticle;
	public GlobalFog fogOfWar;
	// Use this for initialization
	void Start () {
		//gameLevel = 1;
		StartLevel (gameLevel);
		//spawnM = gameObject.GetComponent<SpawnManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		levelText.text = gameLevel.ToString ();
		//if (isLocalPlayer) {
			if (levelActive) {
		
				if (levelSpawnComplete && ZombieHealth.zombieAlive == 0) {
					levelCompleted = true;

				}
				
		
			}
			if (levelCompleted) {
				gameLevel += difficulty;
				StartLevel (gameLevel);
		
		
			}
		//}
	}


	public void StartLevel(int level){
		//Debug.Break ();
		levelActive = true;
		levelCompleted = false;
		levelSpawnComplete = false;
		Debug.Log ("LevelStarted");
		int temp = (level - 1) % 15;
		if (temp < 5) {
			SetDay ();
		} else if (temp < 10) {
			SetNight ();
		} else {
			SetDarkness ();
		}

		spawnM.SetLevel (level);
		//spawnM.SpawnZombies ();
		StartCoroutine (spawnM.SpawnZombies ());





	}

	public void SetDay(){
		RenderSettings.ambientIntensity = 1f;
		fogParticle.alpha = 0;
		fogOfWar.enabled = false;
	
	}
	public void SetNight(){
		RenderSettings.ambientIntensity = 0.4f;
		fogParticle.alpha = 0.6f;
		fogOfWar.enabled = true;
	}
	public void SetDarkness(){
		RenderSettings.ambientIntensity = 0.4f;
		fogParticle.alpha = 0.9f;
		fogOfWar.enabled = true;
	}


	public void SetDifficulty(int diff){
		difficulty = diff;
	
	}

}
