using UnityEngine;
using System.Collections;

public class OurCar : MonoBehaviour {

	public WheelControl rightWheel;
	public WheelControl leftWheel;
	
	public float maxSpeed = 100;

	public Material brakeLights;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		/*if (!Physics.Raycast(transform.position, -transform.up, 5f)) {
			return;
		}*/


		if(rigidbody.velocity.magnitude < maxSpeed){
			leftWheel.UpdateWheelForce();
			rightWheel.UpdateWheelForce();
		}

	}

	public void SetThrottle(float value) {

		if (value < 0) {
			brakeLights.SetFloat("_Intensity", Mathf.Abs(value));
		} else {
			brakeLights.SetFloat("_Intensity", 0);
		}

		float wheelSpeed = value * 100;

		leftWheel.SetWheelSpeed(wheelSpeed);
		rightWheel.SetWheelSpeed(wheelSpeed);
	}

	public void SetSteer(float value) {
		value = Mathf.Clamp(value, -45, 45);

		leftWheel.SetTurnAngle(value);
		rightWheel.SetTurnAngle(value);
	}
}
