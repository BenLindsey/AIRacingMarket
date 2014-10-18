using UnityEngine;
using System.Collections;

public class OurCar : MonoBehaviour {

	public Transform centerOfMass;

    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;
    private Transform[] steeringTransforms;
    private Transform[] allTransforms;

    private float throttle;
    private float steer;

	public Material brakeLights;

	// Use this for initialization
	void Start () {

		rigidbody.centerOfMass = centerOfMass.localPosition;

        steeringTransforms = new Transform[] { frontLeftTransform,
            frontRightTransform };
        allTransforms = new Transform[] { frontRightTransform,
            frontRightTransform, rearLeftTransform, rearRightTransform };
	}

	// Update is called once per frame
    void FixedUpdate() {

        if (throttle < 0) {
            brakeLights.SetFloat("_Intensity", Mathf.Abs(throttle / 100));
        }
        else {
            brakeLights.SetFloat("_Intensity", 0);
        }

        // Set the power of the front wheels.
        frontLeft.motorTorque = throttle;
        frontRight.motorTorque = throttle;

        // Steer the front wheels.
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;

        // Rotate the front wheels around the y axis to show steering.
        frontLeftTransform.localEulerAngles = new Vector3(frontLeftTransform.localEulerAngles.x,
            frontLeft.steerAngle, frontLeftTransform.localEulerAngles.z);
        frontRightTransform.localEulerAngles = new Vector3(frontRightTransform.localEulerAngles.x,
            frontRight.steerAngle, frontRightTransform.localEulerAngles.z);

        // Cumulatively rotate the wheel around the x axis to show speed.
        // Magic number: rpm / 60 * 360 * fixedDeltaTime.
        frontLeftTransform.Rotate(frontLeft.rpm * 6 * Time.fixedDeltaTime, 0, 0);
        frontRightTransform.Rotate(frontRight.rpm * 6 * Time.fixedDeltaTime, 0, 0);
        rearLeftTransform.Rotate(rearLeft.rpm * 6 * Time.fixedDeltaTime, 0, 0);
        rearRightTransform.Rotate(rearRight.rpm * 6 * Time.fixedDeltaTime, 0, 0);
    }

	public void SetThrottle(float value) {
        throttle = value * 100;
	}

	public void SetSteer(float value) {
        steer = Mathf.Clamp(value, -45, 45);
	}
}
