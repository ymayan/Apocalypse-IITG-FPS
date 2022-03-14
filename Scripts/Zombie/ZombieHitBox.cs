using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHitBox : MonoBehaviour {

	public string hitBoxType = "Head";
	public float vulnerability = 1.0f;

	private ZombieAttack zA;
	private ZombieHealth zH;
	private GameObject zombie;
	void Start () {
		zombie = FindParentWithTag (this.gameObject, "Zombie");
		zA = zombie.GetComponent<ZombieAttack> ();
		zH = zombie.GetComponent<ZombieHealth> ();
	}

	void Update () {
		
	}
	public int TakeHit(float damage){
		

		float effectiveDamage = damage * vulnerability;
		return zH.TakeDamage ((int)effectiveDamage);
	}

	void OnTriggerEnter(Collider c){
		//Debug.Log("HB Trigger Enter");
		GameObject player;
		if(c.tag == "PlayerHitBox")
		{
			player = FindParentWithTag (c.gameObject, "Player");
			zA.MyTriggerEnter (player);
		}

		
	}
	void OnTriggerExit(Collider c){
		//Debug.Log("HB Trigger Exit");
		GameObject player;
		if(c.tag == "PlayerHitBox")
		{
			player = FindParentWithTag (c.gameObject, "Player");
			zA.MyTriggerExit (player);
		}

	}


	public static GameObject FindParentWithTag(GameObject childObj,string tag){
		Transform t = childObj.transform;
		while (t.parent != null) {
			if (t.parent.tag == tag) {
				return t.parent.gameObject;
			}
			t = t.parent.transform;
		
		}
		return null;
	
	}
}


