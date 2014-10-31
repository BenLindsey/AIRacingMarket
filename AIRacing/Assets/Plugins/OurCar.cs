using UnityEngine;
using System.Collections.Generic;

public class OurCar : MonoBehaviour {

    private class Wheel {
        public WheelCollider collider;
        public Transform transform;
        public Vector3 originalPosition;
        public bool canSteer;
        public bool isPowered;

        public TrailRenderer currentSkidmark;
    }

	public Transform centerOfMass;

    // Wheel meshes.
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    public AntiRollBar frontAntiRoll;
    public AntiRollBar rearAntiRoll;

    public Material brakeLights;
    public Material skidmark;

    // Physics variables.
    public float downwardsForce = 0;
    public float suspensionDistance = 0.1f;
    public float suspensionFrontForce = 1800;
    public float suspensionRearForce = 900;
    public float suspensionDamper = 5;

    private Wheel[] wheels;

    private float throttle = 0;
	private float brake = 0;
    private float steer = 0;

	// Use this for initialization
	void Start () {

		rigidbody.centerOfMass = centerOfMass.localPosition;

        SetupWheels();

        frontAntiRoll.leftWheel = wheels[0].collider;
        frontAntiRoll.rightWheel = wheels[1].collider;
        frontAntiRoll.antiRoll = suspensionFrontForce;

        rearAntiRoll.leftWheel = wheels[2].collider;
        rearAntiRoll.rightWheel = wheels[3].collider;
        rearAntiRoll.antiRoll = suspensionRearForce;
	}

	// Update is called once per frame
    void FixedUpdate() {

		float maxBrakeForce = -Mathf.Min(throttle, -brake);

        brakeLights.SetFloat("_Intensity", Mathf.Abs(maxBrakeForce / 100f));

        foreach (Wheel wheel in wheels) {
            UpdateWheel(wheel);
        }

        // Messy hack, force the car down on the road to reduce flips.
        rigidbody.AddForceAtPosition(Vector3.down * downwardsForce,
            centerOfMass.position);
    }

    // Adds a wheel collider for each wheel transform.
    private void SetupWheels() {

        wheels = new Wheel[4];

        bool frontWheelDrive = true;
        bool rearWheelDrive = !frontWheelDrive; // Both can be set to true.

        wheels[0] = SetupWheel(frontLeftTransform, frontWheelDrive, true);
        wheels[1] = SetupWheel(frontRightTransform, frontWheelDrive, true);
        wheels[2] = SetupWheel(rearLeftTransform, rearWheelDrive, false);
        wheels[3] = SetupWheel(rearRightTransform, rearWheelDrive, false);
    }

    // Creates the wheel object and its collider from the transform.
    private Wheel SetupWheel(Transform wheelTransform, bool isPowered, bool isFront) {

        GameObject colliderObject = new GameObject(wheelTransform.name + " Collider");
        colliderObject.transform.position = wheelTransform.position;
        colliderObject.transform.parent = wheelTransform.parent.parent;
        colliderObject.transform.rotation = wheelTransform.rotation;

        // Create the collider and set the wheel's radius, friction and suspension.
        WheelCollider collider = colliderObject.AddComponent<WheelCollider>();
        collider.radius = wheelTransform.GetComponentsInChildren<Transform>()[1]
            .renderer.bounds.size.y / 2;
        collider.steerAngle = 0;

        WheelFrictionCurve frictionCurve = new WheelFrictionCurve();
        frictionCurve.extremumSlip   = 1.0f;  // Default values apart from stiffness.
        frictionCurve.extremumValue  = 20000.0f;
        frictionCurve.asymptoteSlip  = 2.0f;
        frictionCurve.asymptoteValue = 10000.0f;
        frictionCurve.stiffness      = 0.04f;
        collider.sidewaysFriction    = frictionCurve;

        JointSpring suspension = new JointSpring();
        suspension.spring = (isFront) ? suspensionFrontForce
                                      : suspensionRearForce;
        suspension.damper             = suspensionDamper;
        suspension.targetPosition     = 0;
        collider.suspensionSpring     = suspension;
        collider.suspensionDistance   = suspensionDistance;

        Wheel wheel = new Wheel();
        wheel.collider  = collider;
        wheel.transform = wheelTransform;
        wheel.canSteer  = isFront;
        wheel.isPowered = isPowered;
        wheel.originalPosition = wheelTransform.parent.localPosition;

        // Move the wheel down according to the height of the suspension.
        wheelTransform.position += Vector3.down * suspensionDistance;

        return wheel;
    }

    private void UpdateWheel(Wheel wheel) {

		wheel.collider.brakeTorque = brake;

        // Update the power of the wheel.
        if (wheel.isPowered) {

            wheel.collider.motorTorque = throttle;
        }

        // Update the steering of the wheel.
        if (wheel.canSteer) {

            wheel.collider.steerAngle = steer;

            // Rotate the front wheel around the y axis to show steering.
            wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x,
                wheel.collider.steerAngle, wheel.transform.localEulerAngles.z);
        }

		// Cumulatively rotate the wheel around the x axis to show speed.
		// Magic number: rpm / 60 * 360 * fixedDeltaTime.
		wheel.transform.Rotate(wheel.collider.rpm * 6 * Time.fixedDeltaTime, 0, 0);

        //WheelHit hit;
        //float a = (wheel.collider.GetGroundHit(out hit))
        //    ? (-wheel.transform.InverseTransformPoint(hit.point).y - wheel.collider.radius)
        //    : 1;
        //// TODO: Update the position of the wheel according to the suspension.
        //wheel.transform.parent.localPosition
        //    = wheel.originalPosition + Vector3.down * a * wheel.collider.suspensionDistance;

        UpdateSkidmarks(wheel);
    }

    private void UpdateSkidmarks(Wheel wheel) {

        const float minSkidSpeed = 1.0f;

        // Check if the wheel is on the ground and skidding enough to draw a skidmark.
        WheelHit hit;
        bool isWheelCurrentlySllpping = wheel.collider.GetGroundHit(out hit)
            && (hit.sidewaysSlip > minSkidSpeed || hit.forwardSlip > minSkidSpeed);

        // If the wheel has now stopped slipping, move the trail renderer out of the car
        // to stop drawing new skidmarks.
        if (wheel.currentSkidmark != null && !isWheelCurrentlySllpping) {

            wheel.currentSkidmark.transform.parent = null;
            wheel.currentSkidmark = null;
        }

        // If the wheel has just started skidding, create a new trail renderer on a new
        // game object as a child of the wheel.
        else if (wheel.currentSkidmark == null && isWheelCurrentlySllpping) {

            // Create a new game object on the bottom of the wheel to hold the trail renderer.
            GameObject trailRendererObject = new GameObject("Skidmark");
            trailRendererObject.transform.position = new Vector3(wheel.transform.position.x,
                wheel.transform.position.y - wheel.collider.radius / 2, wheel.transform.position.z);
            trailRendererObject.transform.parent = wheel.transform.parent;

            // Setup the new trail renderer.
            wheel.currentSkidmark = trailRendererObject.AddComponent<TrailRenderer>();
            wheel.currentSkidmark.time = 30;
            wheel.currentSkidmark.startWidth = 0.3f;
            wheel.currentSkidmark.endWidth = wheel.currentSkidmark.startWidth;
            wheel.currentSkidmark.material = skidmark;
            wheel.currentSkidmark.autodestruct = true; // Remove the game object when it stops drawing.
        }
    }

    public void SetThrottle(float value) {
        throttle = value * 80;
	}

	public void SetSteer(float value) {
        steer = Mathf.Clamp(value, -45, 45);
	}

	public void SetBrake(float value) {
		brake = Mathf.Max(value, 0);
	}
    
    private float GetMaxSteeringAngle(float speed) {

        float topSpeed = 35;
        float minimumTurn = 15;
        float maximumTurn = 45;

	    if (speed > topSpeed / 2) {
		    return minimumTurn;
        }
	
	    float speedIndex = 1 - (speed / (topSpeed / 2));
	    return minimumTurn + speedIndex * (maximumTurn - minimumTurn);
    }
}
