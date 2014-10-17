using UnityEngine;
using System.Collections;

public class WheelControl : MonoBehaviour {

	float steerAngle;
	float wheelSpeed;

	Transform car;

	// Use this for initialization
	void Start () {
		car = transform.parent;
	}

	public void FixedUpdate () {
		float currentAngle =  Vector3.Angle(car.forward, transform.forward);

		if (transform.forward.x < car.forward.x) {
			currentAngle = -currentAngle;
		}

		Debug.Log("Current: " + currentAngle + " Target: " + steerAngle);

		//float difference = steerAngle - currentAngle; 

		//float mag = difference * 0.1f;

		//Debug.Log(difference);
		rigidbody.MoveRotation(Quaternion.AngleAxis(steerAngle - 135, car.up));

	}

	public void UpdateWheelForce() {
		rigidbody.AddForce(transform.forward * wheelSpeed);
	}

	public void SetTurnAngle(float value) {
		steerAngle = value;
	}

	public void SetWheelSpeed(float value) {
		wheelSpeed = value;
	}
}
