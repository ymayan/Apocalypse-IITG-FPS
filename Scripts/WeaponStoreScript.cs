using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class WeaponStoreScript : MonoBehaviour {

	public WeaponInventory wI;
	public PlayerVitals pS;
	public Text playerCoins;
	public GameObject failPopup;
	public GameObject successPopup;
	//
	public void buyM4(){
		if (pS.score >= 160) {
			wI.BuyWeapon (3);
			pS.score -= 160;
			SuccessBuy ();
		} else {
			FailBuy ();
		
		}
	}
	public void buyShotgun(){
		if (pS.score >= 200) {
			wI.BuyWeapon (4);
			pS.score -= 200;
			SuccessBuy ();
		}else {
			FailBuy ();

		}
	}
	public void buySniper(){
		if (pS.score >= 300) {
			wI.BuyWeapon (2);
			pS.score -= 300;
			SuccessBuy ();
		}else {
			FailBuy ();

		}
	}
	public void buyAK(){
		if (pS.score >= 120) {
			wI.BuyWeapon (1);
			pS.score -= 120;
			SuccessBuy ();
		}else {
			FailBuy ();

		}
	}
	public void buyHealth(){
		if (pS.score >= 60) {
			pS.hitPoints = 100;
			pS.score -= 60;
			SuccessBuy ();
		}else {
			FailBuy ();

		}
	}
	public void buyAmmo(){
		if (pS.score >= 60) {
			wI.BuyAmmo ();
			pS.score -= 60;
			SuccessBuy ();
		}else {
			FailBuy ();

		}
	}


	// Use this for initialization
	void Start () {
		
	}

	public void FailBuy(){
		successPopup.SetActive (false);
		failPopup.SetActive (true);
	}
	public void SuccessBuy(){
		successPopup.SetActive (true);
		failPopup.SetActive (false);
	}
	// Update is called once per frame
	void Update () {
		playerCoins.text = pS.score.ToString ();
	}
}
