using UnityEngine;
using System.Collections;

public class WheelControl : MonoBehaviour {

	float steerAngle;

	Transform car;

	public WheelCollider wheelCollider;

	// Use this for initialization
	void Start () {
		car = transform.parent;
	}

	public void FixedUpdate () {
		float currentAngle =  Vector3.Angle(car.forward, transform.forward);

		if (transform.forward.x < car.forward.x) {
			currentAngle = -currentAngle;
		}

		//Debug.Log("Steer: " + steerAngle);

        wheelCollider.steerAngle = steerAngle;

        rigidbody.MoveRotation(Quaternion.AngleAxis(steerAngle, car.up) * car.rotation);
	}
	
	public void SetTurnAngle(float value) {
		steerAngle = value;
	}

	public void SetWheelSpeed(float value) {
		wheelCollider.motorTorque = value;
	}
}
