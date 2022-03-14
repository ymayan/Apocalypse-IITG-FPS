using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum WeaponSlot { Melee, Primary, Secondary }

public class WeaponInventory : MonoBehaviour {

	//GUI
	public Text ammoText;
	public Text magText;

	public GameObject weaponStore;


	public GameObject[] weaponsInUse;
	public GameObject[] weaponsInGame;
	public GameObject[] meleeWeaponsInGame;
	public Rigidbody[] worldModels;
	public Rigidbody[] meleeWorldModels;

	public RaycastHit hit;
	private float dis = 3.0f;
	public LayerMask layerMaskWeapon;
	public LayerMask layerMaskAmmo;

	public Transform dropPosition;

	private float switchWeaponTime = 0.25f;
	//[HideInInspector]
	public bool canSwitch = true;
	[HideInInspector]
	public bool showWepGui = false;
	[HideInInspector]
	public bool showAmmoGui = false;
	public bool showWSGui = false;
	private bool equipped = false;
	[HideInInspector]
	public int weaponToSelect;
	//public int weaponSelected=0;
	[HideInInspector]
	public int setElement;
	[HideInInspector]
	public int weaponToDrop;

	public GUISkin mySkin;
	public AudioClip pickupSound;
	public AudioSource aSource;
	public PlayerVitals hs;
	private string textFromPickupScript = "";
	private string notes = "";
	private string note = "Press key <color=#88FF6AFF> << E >> </color> to pick up Ammo";
	private string wrongType = "Select appropriate weapon to pick up";
	public int selectMelee = 0;
	public int selectPrimary = 1;
	public int selectSecondary = 2;

	void Start()
	{
		
		for (int h = 0; h < worldModels.Length; h++)
		{
			weaponsInGame[h].SetActive(false);
		}
		for (int h = 0; h < meleeWorldModels.Length; h++)
		{
			meleeWeaponsInGame[h].SetActive(false);
		}
		meleeWeaponsInGame [selectMelee].SetActive (true);
		weaponsInUse[0] = weaponsInGame[0];
		weaponsInUse[1] = weaponsInGame[selectPrimary];
		weaponsInUse[2] = weaponsInGame[selectSecondary];

		weaponToSelect = 0;
		StartCoroutine(DeselectWeapon());
	}
	void Update()
	{
		//if (isLocalPlayer) {
			SetGUI ();
			//if (Cursor.lockState == CursorLockMode.None) return;

			if (Input.GetKeyDown ("1") && weaponsInUse.Length >= 1 && canSwitch && weaponToSelect != 0) {
				StartCoroutine (DeselectWeapon ());
				weaponToSelect = 0;

			} else if (Input.GetKeyDown ("2") && weaponsInUse.Length >= 2 && canSwitch && weaponToSelect != 1) {
				StartCoroutine (DeselectWeapon ());
				weaponToSelect = 1;

			} else if (Input.GetKeyDown ("3") && weaponsInUse.Length >= 3 && canSwitch && weaponToSelect != 2) {
				StartCoroutine (DeselectWeapon ());
				weaponToSelect = 2;

			}

			if (Input.GetAxis ("Mouse ScrollWheel") > 0 && canSwitch) {
				weaponToSelect++;
				if (weaponToSelect > (weaponsInUse.Length - 1)) {
					weaponToSelect = 0;
				}
				StartCoroutine (DeselectWeapon ());
			}

			if (Input.GetAxis ("Mouse ScrollWheel") < 0 && canSwitch) {
				weaponToSelect--;
				if (weaponToSelect < 0) {
					weaponToSelect = weaponsInUse.Length - 1;
				}
				StartCoroutine (DeselectWeapon ());
			}

			Vector3 pos = transform.parent.position;
			Vector3 dir = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (pos, dir, out hit, dis, layerMaskWeapon)) {

			WeaponState pre = hit.transform.GetComponent<WeaponState> ();
			bool isMelee = pre.isMelee;
			if (isMelee) {
				
				setElement = pre.meleeIndex;
				if (selectMelee != setElement) {
					equipped = false;
				} else {
					equipped = true;
				}

			} else {
				setElement = pre.weaponIndex;
				if (weaponsInUse [1] != weaponsInGame [setElement] && weaponsInUse [2] != weaponsInGame [setElement]) {
					equipped = false;
				} else {
					equipped = true;
				}
			}
			showWepGui = true;

			//Debug.Log ("RayCastG");
			if (canSwitch && !equipped) {
				//Debug.Log ("Can Switch");
				if (Input.GetKeyDown ("e")) {
					if (weaponToSelect != 0 && !pre.isMelee) {
						DropWeapon (weaponToDrop);
						StartCoroutine (DeselectWeapon ());
						weaponsInUse [weaponToSelect] = weaponsInGame [setElement];
						Destroy (hit.transform.gameObject);
					} else if (weaponToSelect == 0 && pre.isMelee) {
						
						DropWeapon (0);
						Debug.Log ("To Drop");
						Debug.Log (selectMelee);
						meleeWeaponsInGame [selectMelee].SetActive (false);
						selectMelee = setElement;
						Debug.Log ("To Pick");
						Debug.Log (selectMelee);
						meleeWeaponsInGame [selectMelee].SetActive (true);

						//StartCoroutine (DeselectWeapon ());
						//weaponsInUse [weaponToSelect] = weaponsInGame [setElement];
						Destroy (hit.transform.gameObject);
					
					}
				}
			}

		} else {
			showWepGui = false;
		}
	
		if (Physics.Raycast (pos, dir, out hit, dis, layerMaskAmmo)) {
			
			//if (hit.transform.CompareTag ("WeaponStore")) {
				showWSGui = true;
				if (Input.GetKeyDown ("e")) {
					showWSGui = false;
					weaponStore.SetActive (true);
					Time.timeScale = 0;
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
					showWSGui = false;
				}
			//}
		} else {
			showWSGui = false;
		}		

		/*
		if (Physics.Raycast (pos, dir, out hit, dis, layerMaskAmmo)) {
			showAmmoGui = true;
			if (hit.transform.CompareTag ("Ammo")) {
				PickupObj pickupGO = hit.transform.GetComponent<PickupObj> ();

				//bullets/magazines
				if (pickupGO.pickupType == PickupType.Magazines) {
					Weapon mags = weaponsInUse [weaponToSelect].transform.GetComponentInChildren<Weapon> ();
					if (mags == null) {
						textFromPickupScript = "";
						return;
					}
					if (mags.firstMode != fireMode.launcher) {
						notes = "";
						textFromPickupScript = note;
						if (Input.GetKeyDown ("e")) {
							if (mags.ammoMode == Ammo.Magazines)
								mags.ammoLeftRead += pickupGO.amount;
							else
								mags.ammoLeftRead += pickupGO.amount * mags.bulletsPerMagRead;

							aSource.PlayOneShot (pickupSound, 0.3f);
							Destroy (hit.transform.gameObject);
						}
					} else {
						textFromPickupScript = pickupGO.AmmoInfo;
						notes = wrongType;
					}
				}
					

				//health
				if (pickupGO.pickupType == PickupType.Health) {
					textFromPickupScript = pickupGO.AmmoInfo;
					notes = "";
					if (Input.GetKeyDown ("e")) {
						hs.Medic (pickupGO.amount);
						aSource.PlayOneShot (pickupSound, 0.3f);
						Destroy (hit.transform.gameObject);
					}
				}
			}

		} else {
			showAmmoGui = false;
		}*/
		//}
	}
	public void BuyWeapon(int index){
		DropWeapon (weaponToDrop);
		//StartCoroutine (DeselectWeapon ());
		weaponsInUse[weaponToSelect].SetActive(false);
		weaponsInUse [weaponToSelect] = weaponsInGame [index];
		weaponsInUse[weaponToSelect].SetActive(true);

	
	}
	public void BuyAmmo(){
		
		weaponsInUse [1].GetComponent<WeaponController> ().wep.refillAmmo ();
		weaponsInUse [2].GetComponent<WeaponController> ().wep.refillAmmo ();

	}

	void SetGUI(){
		WeaponState wp = weaponsInUse [weaponToSelect].GetComponent<WeaponState> ();
		if (!wp.isMelee) {
			ammoText.text = wp.bulletsLeft.ToString ();
			magText.text = wp.ammoLeft.ToString ();
		}
	}
	void OnGUI()
	{
		//GUI.skin = mySkin;

		if (showWepGui)
		{
			if (!equipped)
				GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Press key <color=#88FF6AFF> << E >> </color> to pickup weapon", mySkin.customStyles[1]);
			else
				GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Weapon is already equipped", mySkin.customStyles[1]);
		}
		if (showWSGui)
		{
			if(Time.timeScale > 0)
				GUI.Label(new Rect(Screen.width / 2 - 300 , Screen.height - (Screen.height / 1.4f), 600,60), "<color=#88FF6AFF> Press key  << E >> to open Store</color> ", mySkin.customStyles[1]);

		}

		if (showAmmoGui)
			GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 200), notes + "\n" + textFromPickupScript, mySkin.customStyles[1]);
	}

	IEnumerator DeselectWeapon()
	{
		//if (weaponsInUse[weaponSelected].activeInHierarchy && weaponsInUse [weaponSelected].GetComponent<Weapon> ().reloading)
			//yield break;
		canSwitch = false;

		for (int i = 0; i < weaponsInUse.Length; i++)
		{
			weaponsInUse[i].SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
			weaponsInUse[i].gameObject.SetActive(false);
		}

		yield return new WaitForSeconds(switchWeaponTime);
		SelectWeapon(weaponToSelect);
		//yield return new WaitForSeconds(switchWeaponTime);
		canSwitch = true;
	}
	void SelectWeapon(int i)
	{
		weaponsInUse[i].gameObject.SetActive(true);
		weaponsInUse[i].SendMessage("DrawWeapon", SendMessageOptions.DontRequireReceiver);
		WeaponState temp = weaponsInUse[i].transform.GetComponent<WeaponState>();
		weaponToDrop = temp.weaponIndex;
		//weaponSelected = weaponToDrop;
	}

	void DropWeapon(int index)
	{
		if (index == 0) {
			Rigidbody drop = Instantiate (meleeWorldModels [selectMelee], dropPosition.transform.position, dropPosition.transform.rotation) as Rigidbody;
			drop.AddRelativeForce (0, 250, Random.Range (100, 200));
			drop.AddTorque (-transform.up * 40);


			return;
		}
			
		for (int i = 0; i < worldModels.Length; i++) {
			if (i == index) {
				Rigidbody drop = Instantiate (worldModels [i], dropPosition.transform.position, dropPosition.transform.rotation) as Rigidbody;
				drop.AddRelativeForce (0, 250, Random.Range (100, 200));
				drop.AddTorque (-transform.up * 40);
			}
		}




	}

	public void EnterWater()
	{
		canSwitch = false;
		for (int i = 0; i < weaponsInUse.Length; i++)
		{
			weaponsInUse[i].SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
			weaponsInUse[i].gameObject.SetActive(false);
		}
	}

	public void ExitWater()
	{
		canSwitch = true;
		SelectWeapon(weaponToSelect);
	}

}
