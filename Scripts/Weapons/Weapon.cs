using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ReloadSound
{
    public string name = "Mag out";
    public AudioClip clip;
    public float length;
}

public enum fireMode { none, melee, semi, auto, burst, shotgun, launcher }
public enum Ammo { Magazines, Bullets }
public enum Aim { Simple, Sniper }

public class Weapon : MonoBehaviour
{
	public fireMode currentMode = fireMode.semi;
	public fireMode firstMode = fireMode.semi;
	public fireMode secondMode = fireMode.burst;
	public Ammo ammoMode = Ammo.Magazines;
	public Aim aimMode = Aim.Simple;

	private float scopeTime;
	private bool inScope = false;
	public Texture scopeTexture;
	public GameObject scopeImage;

	[Header("Burst Settings")]
	public int shotsPerBurst = 3;
	public float burstTime = 0.07f;

	[Header("Shotgun Settings")]
	public int pelletsPerShot = 10;
	/*
	[Header("Launcher")]
	public GameObject projectilePrefab;
	public float projectileSpeed = 30.0f;
	public float projectileGravity = 0.0f;
	public int projectiles = 20;
	public Transform launchPosition;
	public bool reloadProjectile = false;
	public AudioClip soundReloadLauncher;
	public Renderer rocket = null;
*/
    public Animation anim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public AnimationClip reloadEmptyAnim;
    public AnimationClip drawAnim;

    #region bools
    public bool reloading;
    public bool[] canAims;
    private bool canAim;
    public bool[] canReloads;
    private bool canReload;
    public bool[] canFires;
    private bool canFire;
	private bool isFiring = false;
	private bool bursting = false;
    #endregion

    #region stats
	public string weaponType = "Rifle";
    public float fireRate = 0.1f;
    public float timer = 0;
	public float aimSpeed = 0.25f;
	public float zoomSpeed = 0.5f;
	private float nextFireTime = 0.0f;
    [SerializeField]
    protected int bulletsLeft = 30;
    [SerializeField]
    protected int bulletsPerMag = 30;
    [SerializeField]


    protected int ammoLeft = 10;
	public int maxAmmo = 30;
    public float range = 2000;
    public float damageMin = 10;
    public float damageMax = 20;
    public Transform bulletGo;
    public LayerMask hitLayers;
    public GameObject blood;
    public GameObject concrete;
    public GameObject wood;
    public GameObject metal;
    public GameObject dirt;
    #endregion

    #region readOnly
    public int bulletsLeftRead = 30;
    public int bulletsPerMagRead = 30;
    public int ammoLeftRead = 10;
    #endregion


    #region components
    public CharacterValues cv;
    public PlayerAnimations pa;
    #endregion

    #region sound
    public AudioSource localSource;
    public AudioClip fireSound;
	public AudioClip[] missSound;
    public ReloadSound[] drawSound;
    public ReloadSound[] reloadSounds;
    public ReloadSound[] reloadSoundsEmpty;
    #endregion

    #region ads
    public Camera cam;
    public bool aiming;
    public float hipFov = 75;
    public float aimFov = 55;
    private float curFov = 75;
    public Vector3 hipPos;
    public Vector3 crouchPos;
    public Vector3 aimPos;
    private Vector3 curPos;
    #endregion

    #region recoil
    public Transform camKB;
    public Transform wepKB;
    public float minKB;
    public float maxKB;
    public float minKBSide;
    public float maxKBSide;
    public float returnSpeed = 5f;
    #endregion

    #region muzzle
    public GameObject muzzle;
    #endregion

    #region crosshair
	public Image crossImage;
    public float sizeMultiplier = 1f;
    public float aimSpread;
    public float basicSpread = 30;
    public float maximumSpread = 100;
    public float spreadReturnTime = 5;
    public float spreadAddPerShot = 5;
    public float spreadTemp;
    private float spread = 30;

    //Crosshair Textures
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;

    #endregion

    #region private
    private Vector2 pivot;
    #endregion

    #region hitMark
    public Texture2D tex;
    public float size = 32;
    private float hitAlpha;
    public AudioClip hitMarkerSound;
    #endregion


	private WeaponState wState;

    void Start()
    {
		
        muzzle.SetActive(false);
		wState = GetComponentInParent<WeaponState> ();
        //if (GetComponent<NetworkView>().isMine)
        //{
            spreadTemp = basicSpread;
            spread = basicSpread;
            StartCoroutine(CheckBools());
            StartCoroutine(Draw());
        //}
        //else
        //{
        //    this.enabled = false;
        //}
    }

	public void refillAmmo(){
		ammoLeft = maxAmmo;
		bulletsLeft = bulletsPerMag;
	}
    void Update()
    {
		
        //if (isLocalPlayer)
        //{
			
			wState.bulletsLeft = bulletsLeft;
			wState.ammoLeft = ammoLeft;
			
            if (hitAlpha > 0) hitAlpha -= Time.deltaTime;
            spread = Mathf.Clamp(spread, 0, maximumSpread);
            if (aiming) spread = aimSpread;
            else spread = Mathf.Lerp(spread, spreadTemp + cv.velMag * 2, Time.deltaTime * 8);
            if (spreadTemp > basicSpread) spreadTemp -= Time.deltaTime * spreadReturnTime;
            pivot = new Vector2(Screen.width / 2, Screen.height / 2);
            bulletsLeftRead = bulletsLeft;
            bulletsPerMagRead = bulletsPerMag;
            ammoLeftRead = ammoLeft;
            camKB.localRotation = Quaternion.Lerp(camKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            wepKB.localRotation = Quaternion.Lerp(wepKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, curFov, Time.deltaTime * 10);
            transform.localPosition = Vector3.Lerp(transform.localPosition, curPos, Time.deltaTime * 10);
            if (Cursor.lockState == CursorLockMode.Locked)
                CheckInput();
            canReloads[1] = true;
            canAims[1] = !cv.running;
            canFires[1] = !cv.running;
            if (aiming)
            {
				scopeTime = Time.time + aimSpeed;
                curFov = aimFov;
                curPos = aimPos;
            }
            else
            {
                curFov = hipFov;
                if (cv.state == 0)
                {
                    curPos = hipPos;
                }
                else if (cv.state == 1)
                {
                    curPos = crouchPos;
                }
            }
    }

    void OnGUI()
	{
		

		if (scopeTexture != null && inScope) {
			
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), scopeTexture, ScaleMode.ScaleToFit);
			crossImage.enabled = false;
		} else {
			if (currentMode != fireMode.melee && Time.timeScale != 0) {
				crossImage.enabled = true;
				float w = crosshairFirstModeHorizontal.width;
				float h = crosshairFirstModeHorizontal.height;
				Rect position1 = new Rect ((Screen.width + w) / 2 + (spread * sizeMultiplier), (Screen.height - h) / 2, w, h);
				Rect position2 = new Rect ((Screen.width - w) / 2, (Screen.height + h) / 2 + (spread * sizeMultiplier), w, h);
				Rect position3 = new Rect ((Screen.width - w) / 2 - (spread * sizeMultiplier) - w, (Screen.height - h) / 2, w, h);
				Rect position4 = new Rect ((Screen.width - w) / 2, (Screen.height - h) / 2 - (spread * sizeMultiplier) - h, w, h);
				if (!aiming) {
					GUI.DrawTexture (position1, crosshairFirstModeHorizontal);   //Right
					GUI.DrawTexture (position2, crosshairFirstModeVertical);     //Up
					GUI.DrawTexture (position3, crosshairFirstModeHorizontal);   //Left
					GUI.DrawTexture (position4, crosshairFirstModeVertical);     //Down
				}
			} else {
				crossImage.enabled = false;
			}
		}





		GUI.color = new Color (1, 1, 1, hitAlpha);
		GUI.DrawTexture (new Rect ((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), tex);


	}

    void CheckInput()
    {

		if (!reloading && Time.time > timer && canFire && Input.GetKey(KeyCode.Mouse0) && bulletsLeft > 0 && Cursor.lockState == CursorLockMode.Locked)
        {
			if (currentMode == fireMode.melee) {
				if (Time.time - fireRate > nextFireTime)
					nextFireTime = Time.time - Time.deltaTime;

				while (nextFireTime < Time.time)
				{
					StartCoroutine(AttackMelee());
					nextFireTime = Time.time + fireRate;
				}

			}
			else if (currentMode == fireMode.semi)
			{
				if (Time.time - fireRate > nextFireTime)
					nextFireTime = Time.time - Time.deltaTime;

				while (nextFireTime < Time.time)
				{
					FireOneShot();
					nextFireTime = Time.time + fireRate;
				}
			}
			else if (currentMode == fireMode.launcher)
			{
				//FireLauncher();
			}
			else if (currentMode == fireMode.burst)
			{
				StartCoroutine(FireBurst());
			}
			else if (currentMode == fireMode.shotgun)
			{
				if (reloading || bulletsLeft <= 0 )
				{
					//if (bulletsLeft == 0)
					//{
					//	StartCoroutine(OutOfAmmo());
					//}
					return;
				}

				int pellets = 0;

				if (Time.time - fireRate > nextFireTime)
					nextFireTime = Time.time - Time.deltaTime;

				if (Time.time > nextFireTime)
				{
					while (pellets < pelletsPerShot)
					{
						FireOnePellet();
						pellets++;
					}
					anim.Rewind(fireAnim.name);
					anim [fireAnim.name].speed = 0.38f;
					anim.Play(fireAnim.name);
					localSource.clip = fireSound;
					localSource.PlayOneShot(fireSound);
					StartCoroutine(MuzzleFlash());
					//StartCoroutine(Kick3(camKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));
					//StartCoroutine(Kick3(wepKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));
					bulletsLeft--;
					nextFireTime = Time.time + fireRate;
				}
			}
            //FireOneShot();
        }
		if (Input.GetKey(KeyCode.Mouse1) && canAim )
		{
			aiming = true;
			if (aimMode == Aim.Sniper)
			{
				if (Time.time >= scopeTime && !inScope)
				{
					inScope = true;
					if(scopeImage)
						scopeImage.SetActive (true);
					Component[] gos = GetComponentsInChildren<Renderer>(true);
					foreach (var go in gos)
					{
						Renderer a = go as Renderer;
						a.enabled = false;
					}
				}
			}

		}
		else
		{
			if (aiming)
			{
				aiming = false;
				//inScope = false;

				//curVect = hipPosition - transform.localPosition;
				if (inScope)
				{
					if(scopeImage)
						scopeImage.SetActive (false);
					inScope = false;
					Component[] go = GetComponentsInChildren<Renderer>(true);
					foreach (var g in go)
					{
						Renderer b = g as Renderer;
						if (b.name != "muzzle_flash")
							b.enabled = true;
					}
				}
			}


		}
		if (!reloading && canReload && ammoLeft > 0 && Input.GetKeyDown(KeyCode.R) && Cursor.lockState == CursorLockMode.Locked)
        {
            reloading = true;
            StartCoroutine(Reload());
        }
    }
	IEnumerator FireBurst()
	{
		int shotCounter = 0;

		if (reloading || bursting || bulletsLeft <= 0)
		{
			//if (bulletsLeft <= 0)
			//{
			//	StartCoroutine(OutOfAmmo());
			//}
			yield break;
		}

		if (Time.time - fireRate > nextFireTime)
			nextFireTime = Time.time - Time.deltaTime;

		if (Time.time > nextFireTime)
		{
			while (shotCounter < shotsPerBurst)
			{
				bursting = true;
				shotCounter++;
				if (bulletsLeft > 0)
				{
					FireOneShot();
				}
				yield return new WaitForSeconds(burstTime);
			}
			nextFireTime = Time.time + fireRate;
		}
		bursting = false;
	}
	void FireOnePellet()
	{

		spreadTemp += spreadAddPerShot;
		//timer = Time.time + fireRate;


		float actualSpread = Random.Range(-spread, spread);
		//Vector3 position = new Vector3(bulletGo.position.x - actualSpread, bulletGo.position.y - actualSpread, bulletGo.position.z);
		Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * spread, Random.Range(-0.01f, 0.01f) * spread, 1));
		RaycastHit hit2;
		if (Physics.Raycast(bulletGo.position, direction, out hit2, range, hitLayers))
		{
			OnHit(hit2);
		}
	}
	//[ClientRpc]
	void RpcMeleeAttackEffects(int type,bool Hit){
		if (!Hit) {
			localSource.clip = missSound [type];
			localSource.PlayOneShot (missSound [type]);
		} else {
			localSource.clip = fireSound;
			localSource.PlayOneShot(fireSound);
		}


	}


	//[Command]
	void CmdAttackMelee (ref bool isHit, Vector3 origin, Vector3 rdirection){
		RaycastHit hit;
	
		if (Physics.Raycast (origin, rdirection, out hit, range, hitLayers)) {
			if (hit.rigidbody) {
				hit.rigidbody.AddForceAtPosition (200 * bulletGo.forward, hit.point);
			}
			if (hit.transform.tag == "Player") {
				Instantiate (blood, hit.point, Quaternion.identity);
				RpcMeleeAttackEffects (0, isHit);

			} else if (hit.transform.tag == "HitBox") {
				Instantiate (blood, hit.point, Quaternion.identity);

				if (!isHit) {
					//bool isKill = false;
					int stars = 0;
					ZombieHitBox hb = hit.transform.GetComponent<ZombieHitBox> ();
					if (hb) {
						//isKill = hb.TakeHit (damageMax);
						stars = hb.TakeHit(damageMax);
					}
					isHit = true;
					RpcMeleeAttackEffects (0, isHit);
					if (stars == 0) {
						WeaponInventory wI = GetComponentInParent<WeaponInventory> ();
						if (wI) {
							wI.hs.kills++;
							wI.hs.score += stars;
						}
					}
				}


			} 


		}
	
	}

	IEnumerator AttackMelee(){
		int attType = Random.Range (0, 3);
		Animator an = GetComponentInChildren<Animator> ();
		an.SetTrigger("Attack");
		an.SetInteger ("AttackType", attType);
		RpcMeleeAttackEffects (attType,false);

		yield return new WaitForSeconds (0.6f);
		Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * spread, Random.Range(-0.01f, 0.01f) * spread, 1));
		//RaycastHit hit;
		bool isHit = false;


		for (int k = 0; k < 10; k++) {
			yield return new WaitForSeconds (0.03f);

			CmdAttackMelee (ref isHit,bulletGo.position, direction);


		}

		an.SetBool ("IsAttacking", false);


	
	}
    void FireOneShot()
    {
		
        spreadTemp += spreadAddPerShot;
		anim.Rewind (fireAnim.name);
		anim [fireAnim.name].speed = 0.38f;
		anim.Play (fireAnim.name);

		localSource.clip = fireSound;
		localSource.PlayOneShot (fireSound);
		StartCoroutine (MuzzleFlash ());

		if (aiming) {
			StartCoroutine (Kick3 (camKB, new Vector3 (-Random.Range (minKB, maxKB)/2, Random.Range (minKBSide, maxKBSide)/2, 0), 0.1f));
			StartCoroutine (Kick3 (wepKB, new Vector3 (-Random.Range (minKB, maxKB)/2, Random.Range (minKBSide, maxKBSide)/2, 0), 0.1f));
		
		} else {
			StartCoroutine (Kick3 (camKB, new Vector3 (-Random.Range (minKB, maxKB), Random.Range (minKBSide, maxKBSide), 0), 0.1f));
			StartCoroutine (Kick3 (wepKB, new Vector3 (-Random.Range (minKB, maxKB), Random.Range (minKBSide, maxKBSide), 0), 0.1f));
		}

        float actualSpread = Random.Range(-spread, spread);
        //Vector3 position = new Vector3(bulletGo.position.x - actualSpread, bulletGo.position.y - actualSpread, bulletGo.position.z);
        Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * spread, Random.Range(-0.01f, 0.01f) * spread, 1));
        RaycastHit hit2;

		if (Physics.Raycast(bulletGo.position, direction, out hit2, range, hitLayers))
		{
			OnHit(hit2);
		}
        bulletsLeft--;
    }

    void DoHitMark()
    {
        hitAlpha = 2;
        //GetComponent<AudioSource>().PlayOneShot(hitMarkerSound, 1f);
    }
    void OnHit(RaycastHit hit)
    {
        if (hit.rigidbody)
        {
            hit.rigidbody.AddForceAtPosition(200 * bulletGo.forward, hit.point);
        }
		if (hit.transform.tag == "Player") {
			Instantiate (blood, hit.point, Quaternion.identity);
			DoHitMark ();
			//if (hit.transform.root.GetComponent<NetworkView>())
			//    hit.transform.root.GetComponent<NetworkView>().RPC("ApplyDamage", RPCMode.AllBuffered, Random.Range(damageMin, damageMax), 1);
		} 
		else if (hit.transform.tag == "HitBox") {
			int stars = 0;
			Instantiate (blood, hit.point, Quaternion.identity);
			DoHitMark ();
			ZombieHitBox hb = hit.transform.GetComponent<ZombieHitBox> ();
			if (hb) {
				stars = hb.TakeHit (damageMax);

			
			}
			if (stars != 0) {
				WeaponInventory wI = GetComponentInParent<WeaponInventory> ();
				if (wI) {
					wI.hs.kills++;
					wI.hs.score += stars;
				}
			
			}
		
		}
        else
        {
			if (hit.transform.tag == "Wood") {
				GameObject theObj = Instantiate (wood, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.up, hit.normal)) as GameObject;
				theObj.transform.parent = hit.transform;
			} else if (hit.transform.tag == "Metal") {
				GameObject theObj = Instantiate (metal, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.up, hit.normal)) as GameObject;
				theObj.transform.parent = hit.transform;
			} else if (hit.transform.tag == "Dirt") {
				GameObject theObj = Instantiate (dirt, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.up, hit.normal)) as GameObject;
				theObj.transform.parent = hit.transform;
			}
            else
            {
                GameObject theObj = Instantiate(concrete, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                theObj.transform.parent = hit.transform;
            }
        }
    }

    IEnumerator Kick3(Transform goTransform, Vector3 kbDirection, float time)
    {
        Quaternion startRotation = goTransform.localRotation;
        Quaternion endRotation = goTransform.localRotation * Quaternion.Euler(kbDirection);
        float rate = 1.0f / time;
        var t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            goTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }
	void OnEnable(){
		//anim.Play ("idle");
		StartCoroutine(CheckBools());
		StartCoroutine(Draw());
		canAims[0] = true;
		canFires[0] = true;
		canReloads[0] = true;
		reloading = false;


	}
	public void DisableScope(){
	
		if(scopeImage)
			scopeImage.SetActive (false);
		//inScope = false;
		Component[] go = GetComponentsInChildren<Renderer>(true);
		foreach (var g in go)
		{
			Renderer b = g as Renderer;
			if (b.name != "muzzle_flash")
				b.enabled = true;
		}
	
	}
    IEnumerator Reload()
	{
		reloading = true;
		canAims [0] = false;
		canFires [0] = false;
		canReloads [0] = false;
		if (bulletsLeft != bulletsPerMag) {
			if (bulletsLeft > 0) {
				if (currentMode == fireMode.melee) {
					//GetComponentInChildren<Animator> ().SetTrigger ("A");
				} else {
					StartCoroutine (ReloadingSound (reloadSounds));
					anim.Rewind (reloadAnim.name);
					anim [reloadAnim.name].speed = 1.38f;
					anim.Play (reloadAnim.name);
				}
				yield return new WaitForSeconds (reloadAnim.length / 1.38f);
				if (ammoMode == Ammo.Magazines) {
					bulletsLeft = bulletsPerMag;
					ammoLeft--;
				} else if (ammoMode == Ammo.Bullets) {
					int ammoneeded = bulletsPerMag - bulletsLeft;
					if (ammoLeft < ammoneeded) {
						bulletsLeft += ammoLeft;
						ammoLeft = 0;
					} else {
						ammoLeft -= ammoneeded;
						bulletsLeft += ammoneeded;
					}

				}
			} else {
				if (currentMode == fireMode.melee) {
					GetComponentInChildren<Animator> ().SetTrigger ("Draw");
				} else {
					StartCoroutine (ReloadingSound (reloadSoundsEmpty));
					anim.Rewind (reloadEmptyAnim.name);
					anim [reloadEmptyAnim.name].speed = 1.38f;
					anim.Play (reloadEmptyAnim.name);
				}
				yield return new WaitForSeconds (reloadEmptyAnim.length / 1.38f);
				if (ammoMode == Ammo.Magazines) {
					bulletsLeft = bulletsPerMag;
					ammoLeft--;
				} else if (ammoMode == Ammo.Bullets) {
					int ammoneeded = bulletsPerMag - bulletsLeft;
					if (ammoLeft < ammoneeded) {
						bulletsLeft += ammoLeft;
						ammoLeft = 0;
					} else {
						ammoLeft -= ammoneeded;
						bulletsLeft += ammoneeded;
					}

				}
			}
		}
	
        canAims[0] = true;
        canFires[0] = true;
        canReloads[0] = true;
        reloading = false;
    }

    IEnumerator ReloadingSound(ReloadSound[] theSound)
    {
        foreach (ReloadSound lol in theSound)
        {
            yield return new WaitForSeconds(lol.length);
            localSource.clip = lol.clip;
            localSource.Play();
        }
    }

    IEnumerator CheckBools()
    {
        CheckAim();
        CheckReload();
        CheckFire();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CheckBools());
    }

    IEnumerator MuzzleFlash()
    {
        muzzle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzle.SetActive(false);
    }

    void CheckAim()
    {
        canAim = false;
        foreach (bool lol in canAims)
        {
            if (!lol) return;
        }
        canAim = true;
    }

    void CheckReload()
    {
        canReload = false;
        foreach (bool lol in canReloads)
        {
            if (!lol) return;
        }
        canReload = true;
    }

    void CheckFire()
    {
        canFire = false;
        foreach (bool lol in canFires)
        {
            if (!lol) return;
        }
        canFire = true;
    }

    IEnumerator Draw()
    {
        canAims[0] = false;
        canFires[0] = false;
        canReloads[0] = false;
        //localSource.clip = drawSound;
        //localSource.Play();
        StartCoroutine(ReloadingSound(drawSound));
		if (currentMode == fireMode.melee) {
			Animator an = GetComponentInChildren<Animator> ();
			if (an)
				an.SetTrigger ("Draw");
		}
		else
        	//anim.Play(drawAnim.name);
        yield return new WaitForSeconds(drawAnim.length);
        canAims[0] = true;
        canFires[0] = true;
        canReloads[0] = true;
    }
	
}
