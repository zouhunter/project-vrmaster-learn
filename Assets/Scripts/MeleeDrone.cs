//This script manages the melee drone. It is responsible for moving the drone towards the player and destroying the drone when it collides with the player

using System.Collections;
using UnityEngine;
using NobleMuffins.TurboSlicer;

public class MeleeDrone : MonoBehaviour 
{
	const string explosionPoolName = "Explosion";	//The name of the explosion prefab in the Object Pool	

	[SerializeField] int pointValue = 100;			//The amount of points this enemy is worth
	[SerializeField] float minTargetHeight = 1f;	//Minimum height the drone will reach
	[SerializeField] float maxTargetHeight = 2.5f;	//Maximum height the drone will reach
	[SerializeField] float minSpeed = 1f;			//Minimum speed the drone will fly
	[SerializeField] float maxSpeed = 2.5f;			//Maximum speed the drone will fly

	Transform target;								//The drone's target
	Rigidbody rigidBody;							//A reference to the rigidbod
	float speed;									//Current speed of the drone
	float initialHeight;							//The initial height the drone will reach
	bool isAlive = true;							//Is the drone currently alive?
	bool heightReached;								//Has the drone reached its initial height?

	void Start()
	{
		//Get references to the rigidbody and set the target to the player
		rigidBody = GetComponent<Rigidbody> ();
		target = GameManager.instance.player;

		//Calculate a random initial height and speed
		speed = Random.Range (minSpeed, maxSpeed);
		initialHeight = Random.Range (minTargetHeight, maxTargetHeight);

		//Start the movement cycle
		StartCoroutine (MovementCycle ());
	}

	IEnumerator MovementCycle()
	{
		//Declare a vector that will be used to determine the direction the drone flies
		Vector3 direction;

		//Initialize the delay. This drone will update every fixed frame
		WaitForFixedUpdate delay = new WaitForFixedUpdate ();

		while (isAlive)
		{
			//Look at the target
			transform.LookAt(target.position);

			//If the drone has reached its initial height, then fly forward. Otherwise, continue to fly up
			if (heightReached)
				direction = transform.forward;
			else
				direction = Vector3.up;

			//Calculate the drone's new position and move it there
			Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;	
			rigidBody.MovePosition (newPosition);

			//If the drone has reached its target height, set heightReached to true
			if (transform.position.y >= initialHeight)
				heightReached = true;			

			//Wait until the next fixed frame
			yield return delay;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		//If the drone has collided with anything other than the player, exit this method
		if (!other.gameObject.CompareTag ("Player"))
			return;

		//The drone has collided with the player. Use the Turber Slicer API to shatter it
		TurboSlicerSingleton.Instance.Shatter (gameObject, 1);

		//Get an explosion effect from the object pool
		ObjectPoolManager.instance.GetObject (explosionPoolName, true, transform.position);
	}

	//This method will be called when the drone is sliced by the Player's sword
	public void Sliced()
	{
		//Tell the Game Manager to add more points
		GameManager.instance.AddPoints(pointValue);
	}
}
