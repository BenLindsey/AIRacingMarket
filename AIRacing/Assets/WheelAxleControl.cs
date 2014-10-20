using UnityEngine;
using System.Collections;

public class WheelAxleControl : MonoBehaviour {

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    public float antiRollForce = 5000.0f;

    private Rigidbody car;

	// Use this for initialization
	void Start () {
        car = transform.parent.rigidbody;
	}

    void Update() {
        Debug.DrawLine(leftWheel.transform.position, rightWheel.transform.position, Color.red);
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

        float antiRollForce = (leftTravel - rightTravel) * antiRollForce;

        if (isLeftWheelGrounded) {
            car.rigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRollForce,
                leftWheel.transform.position);
        }

        if (isRightWheelGrounded) {
            car.rigidbody.AddForceAtPosition(rightWheel.transform.up * antiRollForce,
                rightWheel.transform.position);
        }
	}

    public void setSteer(float value) {
        leftWheel.steerAngle = value;
        rightWheel.steerAngle = value;
    }

    public void setThrottle(float value) {

    }
}
