using UnityEngine;
using System.Collections;

public class WheelControl : MonoBehaviour {

	public WheelCollider wheelCollider;

	// Use this for initialization
	void Start () {
	}

	public void FixedUpdate () {

        // Rotate the the wheel around the y axis to show steering.
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
            wheelCollider.steerAngle, transform.localEulerAngles.z);

        // Cumulatively rotate the wheel around the x axis to show speed.
        // Magic number: rpm / 60 * 360 * fixedDeltaTime.
        transform.Rotate(wheelCollider.rpm * 6 * Time.fixedDeltaTime, 0, 0);
	}

    public void SetTurnAngle(float value) {
        wheelCollider.steerAngle = value;
	}

	public void SetWheelSpeed(float value) {
		wheelCollider.motorTorque = value;
	}
}
