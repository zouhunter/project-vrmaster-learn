//This script records the speed of the sword based on the change of position of the tip over time. This is used for cutting, bullet
//deflection, and audio. This script also demonstrates how to use trigger inputs to play a graphical
//effect on the sword

using UnityEngine;
using UnityEngine.VR;

public class Sword : MonoBehaviour 
{
	const string rightTriggerAxis = "SecondaryIndexTrigger";	//Input mapping for Oculus Touch. Would need remapped for Vive
	const string leftTriggerAxis = "PrimaryIndexTrigger";		//Input mapping for Oculus Touch. Would need remapped for Vive

	[SerializeField] Transform swordTip;		//A reference to the tip of the sword
	[SerializeField] float initialDelay = .5f;	//Initial delay before speed recording begins

	float speed;								//The current speed of the sword
	public float Speed							//A public property to make the speed accessible (read only)
	{
		get{ return speed; }
	}

	Animator anim;								//A reference to the animator component
	Vector3 lastPos;							//The last good position of the sword tip
	bool isReady;								//Is the sword speed ready to be measured?
	bool isRightHand;							//Is this sword in the right hand?

	void Start () 
	{
		//Get a reference to the animator component on the sword child object
		anim = GetComponentInChildren<Animator> ();

		//Determine if this is the right hand or not by examining the parent VRObjectTracking script
		UnityEngine.XR.XRNode node = transform.parent.GetComponent<VRObjectTracking> ().node;
		if (node == UnityEngine.XR.XRNode.RightHand)
			isRightHand = true;

		//Invoke the enable method after a delay. This is done so that the sword speed isn't being measured
		//as soon as the scene starts. After the first frame, VR tracking becomes enabled which will make
		//the sword position jump and have a high speed
		Invoke ("Enable", initialDelay);
	}

	void Update()
	{
		//Look for inputs on the left and right hands. If there is input and they match
		//the hand this sword is in, then play the charge animation. Otherwise stop the
		//charge animation. Note that the Oculus Touch treats the trigger as an axis and so we
		//use GetAxisRaw(). The Vive treats the trigger as a button and you would instead
		//need to use GetButtonDown()
		if ((Input.GetAxisRaw (rightTriggerAxis) > 0f && isRightHand) ||
			(Input.GetAxisRaw(leftTriggerAxis) > 0f && !isRightHand))
			anim.SetBool ("Charged", true);
		else
			anim.SetBool ("Charged", false);
	}

	void FixedUpdate()
	{
		//If the sword isn't ready, exit
		if (!isReady)
			return;

		//Calculate the speed of the sword
		float tempSpeed = Vector3.Distance(swordTip.position, lastPos) / Time.deltaTime;

		//If the speed is effectively 0, exit the method. If FixedUpdate runs twice in between
		//sword motion, a false value of 0 will be calculated. This prevents that from being 
		//recorded
		if (tempSpeed < .0000001f)
			return;

		//Set the speed and last position of the sword tip
		speed = tempSpeed;
		lastPos = swordTip.position;
	}

	void Enable()
	{
		//Enable the sword for speed recording
		lastPos = swordTip.position;
		isReady = true;
	}
}
