using UnityEngine;
using System.Collections;

public class OurCar : MonoBehaviour {

    private class Wheel {
        public WheelCollider collider;
        public Transform transform;
        public bool canSteer;
        public bool isPowered;
    }

	public Transform centerOfMass;

    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    private Wheel[] wheels;

    private float throttle = 0;
    private float steer = 0;

	public Material brakeLights;

	// Use this for initialization
	void Start () {

		rigidbody.centerOfMass = centerOfMass.localPosition;

        SetupWheels();
	}

	// Update is called once per frame
    void FixedUpdate() {

        if (throttle < 0) {
            brakeLights.SetFloat("_Intensity", Mathf.Abs(throttle / 100));
        }
        else {
            brakeLights.SetFloat("_Intensity", 0);
        }

        foreach (Wheel wheel in wheels) {
            UpdateWheel(wheel);
        }
    }

    // Adds a wheel collider for each wheel transform.
    private void SetupWheels() {

        wheels = new Wheel[4];
        wheels[0] = SetupWheel(frontLeftTransform, true, true);
        wheels[1] = SetupWheel(frontRightTransform, true, true);
        wheels[2] = SetupWheel(rearLeftTransform, false, false);
        wheels[3] = SetupWheel(rearRightTransform, false, false);
    }

    // Creates the wheel object and its collider from the transform.
    private Wheel SetupWheel(Transform wheelTransfrom, bool canSteer,
        bool isPowered) {

        GameObject colliderObject = new GameObject(wheelTransfrom.name + " Collider");
        colliderObject.transform.position = wheelTransfrom.position;
        colliderObject.transform.parent = wheelTransfrom.parent;
        colliderObject.transform.rotation = wheelTransfrom.rotation;

        WheelCollider collider = colliderObject.AddComponent<WheelCollider>();
        collider.radius = wheelTransfrom.GetComponentsInChildren<Transform>()[1]
            .renderer.bounds.size.y / 2;

        Wheel wheel = new Wheel();
        wheel.collider = collider;
        wheel.transform = wheelTransfrom;
        wheel.canSteer = canSteer;
        wheel.isPowered = isPowered;

        return wheel;
    }

    private void UpdateWheel(Wheel wheel) {

        // Update the power of the wheel.
        if (wheel.isPowered) {

            wheel.collider.motorTorque = throttle;
            wheel.collider.motorTorque = throttle;
        }

        // Update the steering of the wheel.
        if (wheel.canSteer) {

            wheel.collider.steerAngle = steer;
            wheel.collider.steerAngle = steer;

            // Rotate the front wheel around the y axis to show steering.
            wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x,
                wheel.collider.steerAngle, wheel.transform.localEulerAngles.z);
        }

        // Cumulatively rotate the wheel around the x axis to show speed.
        // Magic number: rpm / 60 * 360 * fixedDeltaTime.
        wheel.transform.Rotate(wheel.collider.rpm * 6 * Time.fixedDeltaTime, 0, 0);
    }

    public void SetThrottle(float value) {
        throttle = value * 100;
	}

	public void SetSteer(float value) {
        steer = Mathf.Clamp(value, -45, 45);
	}
}
