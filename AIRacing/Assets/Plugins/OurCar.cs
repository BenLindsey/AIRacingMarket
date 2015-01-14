using UnityEngine;
using System.Collections.Generic;

public class OurCar : MonoBehaviour {

    private class Wheel {
        public WheelCollider collider;
        public Transform transform;
        public Vector3 originalPosition;
        public bool canSteer;
        public bool isPowered;
        public int lastSkidmarkIndex;
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
    public Material skidmarkMaterial;

    public Camera camera;

    // Physics variables.
    public float downwardsForce = 0;
    public float suspensionDistance = 0.1f;
    public float suspensionFrontForce = 1800;
    public float suspensionRearForce = 900;
    public float suspensionDamper = 5;

    private bool isSkidding = false;
    public bool IsSkidding { get { return isSkidding; } }

    private bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    public bool IsFlipped { get { return 90 <= transform.eulerAngles.z
        && transform.eulerAngles.z <= 270; } }
    private float flipTime = -1;
    private const float MAX_FLIP_TIME = 5;

    private Vector3 lastPosition;
    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private Wheel[] wheels;

    private float throttle = 0;
	private float brake = 0;
    private float steer = 0;
    private const int NUM_WHEELS = 4;

    private string name = ":(";
    public string Name { get { return name; } }

    // Boost variables/components
    public ParticleSystem[] exhausts;

    private Skidmarks skidmarks;

    private const int MAX_BOOST = 300;
    private const int BOOST_MULTIPLIER = 20;
    private int boostCooldown = 0;

    private bool fovIncrease = false;
    private float fov = FOV_NORMAL;
    private const int FOV_NORMAL = 60;
    private const int FOV_MAX = 75;
    private const int FOV_CHANGE_MAX = 150; // fov will increase to this but the camera's fov will be capped at FOV_MAX.
    private const float BOOST_FOV_BASE_CHANGE = (FOV_CHANGE_MAX - FOV_NORMAL) / (2 / 0.01f);
    private const float BOOST_FOV_INC = 5 * BOOST_FOV_BASE_CHANGE / 2; // Spend 2/5 of the time increasing the FOV.
    private const float BOOST_FOV_DEC = 5 * BOOST_FOV_BASE_CHANGE / 3; // Spend the rest of the time decreasing.

	// Use this for initialization
	void Start () {

        skidmarks = FindObjectOfType<Skidmarks>();

        // Create an object with the skidmarks script if no such object exists.
        if (skidmarks == null) {
            GameObject skidmarkObject = new GameObject("Skidmark Object");
            skidmarks = skidmarkObject.AddComponent<Skidmarks>();
            skidmarkObject.GetComponent<MeshRenderer>().material = skidmarkMaterial;
        }

		rigidbody.centerOfMass = centerOfMass.localPosition;

        SetupWheels();

        frontAntiRoll.leftWheel = wheels[0].collider;
        frontAntiRoll.rightWheel = wheels[1].collider;
        frontAntiRoll.antiRoll = suspensionFrontForce;

        rearAntiRoll.leftWheel = wheels[2].collider;
        rearAntiRoll.rightWheel = wheels[3].collider;
        rearAntiRoll.antiRoll = suspensionRearForce;

        camera = transform.parent.GetComponentInChildren<Camera>();
	}

	// Update is called once per frame
    void FixedUpdate() {

		float maxBrakeForce = -Mathf.Min(throttle, -brake);

        brakeLights.SetFloat("_Intensity", Mathf.Abs(maxBrakeForce / 100f));

        isSkidding = false;
        isGrounded = false;
        foreach (Wheel wheel in wheels) {
            UpdateWheel(wheel);
        }

        // Update the moving flag so the HUD can check if this vehicle is active.
        isMoving = rigidbody.position != lastPosition;
        lastPosition = rigidbody.position;

        // Messy hack, force the car down on the road to reduce flips.
        rigidbody.AddForceAtPosition(Vector3.down * downwardsForce,
            centerOfMass.position);

        foreach (ParticleSystem exhaust in exhausts) {
            if (exhaust.startLifetime > 0f) {
                exhaust.startLifetime -= 0.01f;
            }
        }

        // Update the field of view when boosting.
        if (fovIncrease) {
            if (fov >= FOV_CHANGE_MAX) {
                fovIncrease = false;
            } else {
                fov += BOOST_FOV_INC;
            }
        } else if (fov >= FOV_NORMAL) {
            fov -= BOOST_FOV_DEC;
        }
        camera.fieldOfView = Mathf.Min(fov, FOV_MAX);

        if (boostCooldown > 0) {
            boostCooldown--;
        }

        // Check if the car should be flipped over.
        if (IsFlipped) {
            // If the car flipped over in this updated.
            if (flipTime == -1) {
                flipTime = Time.time;
            }
            // Reset the car if it has been flipped over for too long.
            else if (Time.time - flipTime >= MAX_FLIP_TIME) {
                UnFlip();
            }
        } else {
            flipTime = -1;
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
        wheel.lastSkidmarkIndex = -1;

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
            Vector3 eulerAngles = wheel.transform.localEulerAngles;
            wheel.transform.localEulerAngles = new Vector3(eulerAngles.x,
                wheel.collider.steerAngle - eulerAngles.z, eulerAngles.z);
        }

		// Cumulatively rotate the wheel around the x axis to show speed.
		// Magic number: rpm / 60 * 360 * fixedDeltaTime.
        wheel.transform.Rotate(wheel.collider.rpm * 6 * Time.fixedDeltaTime, 0, 0);

        // Update the vertical position of the wheel according to the suspension.
        WheelHit hit;
        bool isTouchingGround = wheel.collider.GetGroundHit(out hit);
        float extension = (isTouchingGround)
            ? (-wheel.collider.transform.InverseTransformPoint(hit.point).y - wheel.collider.radius)
            : wheel.collider.suspensionDistance;
        wheel.transform.localPosition
            = wheel.originalPosition + Vector3.down * extension;

        isGrounded |= isTouchingGround;

        UpdateSkidmarks(wheel, isTouchingGround, hit);
    }

    private void UpdateSkidmarks(Wheel wheel, bool isTouchingGround, WheelHit hit) {

        const float minSkidSpeed = 3.0f;

        // Check if the wheel is on the ground and skidding enough to draw a skidmark.
        float slipSpeed = new Vector2(hit.forwardSlip, hit.sidewaysSlip).magnitude;
        bool isWheelCurrentlySlipping = isTouchingGround && slipSpeed > minSkidSpeed;
        isSkidding |= isWheelCurrentlySlipping;

        if (isWheelCurrentlySlipping) {
            // Draw the mark slightly above the ground to avoid clipping with the
            // road surface at long draw distances. Increase the intensity of the
            // texture with the slip speed.
            wheel.lastSkidmarkIndex = skidmarks.AddSkidMark(
                hit.point + rigidbody.velocity * Time.fixedDeltaTime + 0.1f * hit.normal,
                hit.normal, Mathf.Clamp01((slipSpeed - minSkidSpeed) / 10),
                wheel.lastSkidmarkIndex);
            // TODO: Add smoke effect.
        } else {
            wheel.lastSkidmarkIndex = -1;
        }
    }

    private void UnFlip() {
        Debug.Log("Unflipping " + Name);

        // Move the car up a bit and reset z rotation.
        rigidbody.transform.eulerAngles = new Vector3(rigidbody.transform.eulerAngles.x,
            rigidbody.transform.eulerAngles.y, 0);
        rigidbody.transform.position += 0.5f * Vector3.up;
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
        if (boostCooldown == 0) {
            rigidbody.velocity += rigidbody.transform.forward * BOOST_MULTIPLIER;
            boostCooldown = MAX_BOOST;
            foreach (ParticleSystem exhaust in exhausts) {
                exhaust.startLifetime = 2f;
            }

            fovIncrease = true;
        }
    }

    public float GetTimeToNextBoost() {
        return boostCooldown;
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
