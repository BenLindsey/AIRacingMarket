using UnityEngine;
using System.Collections;

public class AntiRollBar : MonoBehaviour {

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public float antiRoll = 5000.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {

        WheelHit hit;
        float leftTravel = 1.0f;
        float rightTravel = 1.0f;

        bool isLeftWheelGrounded = leftWheel.GetGroundHit(out hit);
        if (isLeftWheelGrounded) {
            leftTravel = (-leftWheel.transform.InverseTransformPoint(hit.point).y
                - leftWheel.radius) / leftWheel.suspensionDistance;
        }

        bool isRightWheelGrounded = rightWheel.GetGroundHit(out hit);
        if (isRightWheelGrounded) {
            rightTravel = (-rightWheel.transform.InverseTransformPoint(hit.point).y
                - rightWheel.radius) / rightWheel.suspensionDistance;
        }

        float antiRollForce = (leftTravel - rightTravel) * antiRoll;

        if (isLeftWheelGrounded) {
            rigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRollForce,
                leftWheel.transform.position);
        }

        if (isRightWheelGrounded) {
            rigidbody.AddForceAtPosition(rightWheel.transform.up * antiRollForce,
                rightWheel.transform.position);
        }
	}
}
