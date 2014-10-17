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

		Debug.Log("Steer: " + steerAngle);

		rigidbody.MoveRotation(Quaternion.AngleAxis(steerAngle, car.up) * car.rotation);
	}

	public void UpdateWheelForce() {
		if (Physics.Raycast(transform.position, -transform.up, 0.6f)) {
			rigidbody.AddForce(transform.forward * wheelSpeed);
		}
	}

	public void SetTurnAngle(float value) {
		steerAngle = value;
	}

	public void SetWheelSpeed(float value) {
		wheelSpeed = value;
	}
}
