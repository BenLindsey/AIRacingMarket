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

    private bool isSkidding = false;
    public bool IsSkidding { get { return isSkidding; } }

    private Wheel[] wheels;

    private float throttle = 0;
	private float brake = 0;
    private float steer = 0;
    private const int NUM_WHEELS = 4;

    private string name = ":(";
    public string Name { get { return name; } }

    private const int MAX_BOOST = 1000;
    private int boostCooldown = MAX_BOOST;

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

        isSkidding = false;
        foreach (Wheel wheel in wheels) {
            UpdateWheel(wheel);
        }

        // Messy hack, force the car down on the road to reduce flips.
        rigidbody.AddForceAtPosition(Vector3.down * downwardsForce,
            centerOfMass.position);

        if (boostCooldown < MAX_BOOST) {
            boostCooldown++;
        }
    }

    public void SetCenterOfMass(Vector3 com) {
        rigidbody.centerOfMass = com;
        centerOfMass.localPosition = com;
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
        wheel.originalPosition = wheelTransform.localPosition;

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

        // Update the vertical position of the wheel according to the suspension.
        WheelHit hit;
        float extension = (wheel.collider.GetGroundHit(out hit))
            ? (-wheel.collider.transform.InverseTransformPoint(hit.point).y - wheel.collider.radius)
            : wheel.collider.suspensionDistance;
        wheel.transform.localPosition
            = wheel.originalPosition + Vector3.down * extension;

        UpdateSkidmarks(wheel);
    }

    private void UpdateSkidmarks(Wheel wheel) {

        const float minSkidSpeed = 3.0f;

        // Check if the wheel is on the ground and skidding enough to draw a skidmark.
        WheelHit hit;
        bool isWheelCurrentlySlipping = wheel.collider.GetGroundHit(out hit)
            && (hit.sidewaysSlip > minSkidSpeed || hit.forwardSlip > minSkidSpeed);
        isSkidding |= isWheelCurrentlySlipping;

        // If the wheel has now stopped slipping, move the trail renderer out of the car
        // to stop drawing new skidmarks.
        if (wheel.currentSkidmark != null && !isWheelCurrentlySlipping) {

            wheel.currentSkidmark.transform.parent = null;
            wheel.currentSkidmark = null;
        }

        // If the wheel has just started skidding, create a new trail renderer on a new
        // game object as a child of the wheel.
        else if (wheel.currentSkidmark == null && isWheelCurrentlySlipping) {

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

    public void SetScriptName(string name) {
        Debug.Log("OurCar is setting name to: \"" + name + "\"");
        this.name = name;
    }

    public void Boost() {
        Debug.Log("Velocity before: " + rigidbody.velocity);

        if (boostCooldown == MAX_BOOST) {
            rigidbody.AddForceAtPosition(rigidbody.transform.forward * 300,
            centerOfMass.position);
        }
        
        Debug.Log("Velocity after: " + rigidbody.velocity);
    }

    public float GetTimeToNextBoost() {
        return MAX_BOOST - boostCooldown;
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
