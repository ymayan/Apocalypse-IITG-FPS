using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable] 
public class ToggleEvent : UnityEvent<bool>{}

public class SetPlayerID : NetworkBehaviour {

	public int player1View;
	public int player2View;
	public GameObject weaponRoot;
	public GameObject hero;

	[SerializeField] ToggleEvent OnToggleShared;
	[SerializeField] ToggleEvent OnToggleLocal;
	[SerializeField] ToggleEvent OnToggleRemote;

	GameObject mainCamera;
	void Start(){
		mainCamera = Camera.main.gameObject;

		EnablePlayer ();

	}

	void DisablePlayer(){
		if (isLocalPlayer) {
			mainCamera.SetActive (true);
		}

		OnToggleShared.Invoke (false);
		if (isLocalPlayer)
			OnToggleLocal.Invoke (false);
		else
			OnToggleRemote.Invoke (false);
	
	}

	void EnablePlayer(){

		if (isLocalPlayer) {
			mainCamera.SetActive (false);
		}

		OnToggleShared.Invoke (true);
		if (isLocalPlayer) {
			Debug.Log ("LocalYeah");
			OnToggleLocal.Invoke (true);
		} else {
			OnToggleRemote.Invoke (true);
		}
	}




	void OnConnectedToServer(){
	
		Debug.Log ("PlayerId : ");
		Debug.Log (Network.player.ToString());
	}

	public override void OnStartLocalPlayer(){
		Debug.Log ("StartLocalPlayer");
		if (Network.isServer) {
			Debug.Log ("ServerBro");
			//Player1
			SetLayersOnAll(weaponRoot,player1View);
			SetLayersOnAll(hero,player2View);
		
		} else {//if (Network.isClient) {
			//Player2
			Debug.Log ("ClientxD");
			SetLayersOnAll(weaponRoot,player2View);
			SetLayersOnAll(hero,player1View);
		
		}
	
	}

	static void SetLayersOnAll(GameObject obj,int layer){
		if (obj != null) {
			foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
				trans.gameObject.layer = layer;
			}
		}
	}
}

