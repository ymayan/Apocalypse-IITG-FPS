using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerMovement : MonoBehaviour {

	public float speed = 6f;            // The speed that the player will move at.

	Vector3 movement;                   // The vector to store the direction of the player's movement.
	bool crouch;
	bool jump;
	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.
	ThirdPersonCharacter charController ;

	void Awake ()
	{
		// Create a layer mask for the floor layer.
		floorMask = LayerMask.GetMask ("Floor");
		charController = GetComponent <ThirdPersonCharacter> ();
		crouch = false;
		jump = false;

	}


	void FixedUpdate ()
	{
		// Store the input axes.
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		// Move the player around the scene.
		Move(h,v);




	}

	void Move (float h, float v)
	{
		// Set the movement vector based on the axis input.
		movement.Set (h, 0f, v);

		// Normalise the movement vector and make it proportional to the speed per second.
		movement = movement.normalized * speed * Time.deltaTime;
		// Move the player to it's current position plus the movement.
		charController.Move(movement,crouch,jump);
	}


}
