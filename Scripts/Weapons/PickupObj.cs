using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { Health, Magazines, Projectiles }
public class PickupObj : MonoBehaviour {

	public PickupType pickupType = PickupType.Health;
	public int amount = 3;
	public string AmmoInfo = "";
}
