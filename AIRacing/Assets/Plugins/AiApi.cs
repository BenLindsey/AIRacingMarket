using UnityEngine;
using System.Collections;

public class AiApi : MonoBehaviour {

	public GameObject thing;
	private GameObject otherThing;

    public LayerMask walls;
    public LayerMask cars;

    public BezierSpline centerLane;
    public int numOfLanesWideFromCenter = 1;
    public float laneWidth = 6;

    public TrackAreaManager trackAreaManager;

    int lane = 0;
    float targetAmountAlongSpline = 0.01f;
    Vector3 targetPoint;

	Transform detector;

	OurCar car;

	// Use this for initialization
	void Start () {
		otherThing = Instantiate(thing) as GameObject;

		car = GetComponent<OurCar>();
		detector = transform.FindChild("Detector");
        targetPoint = centerLane.GetPoint(targetAmountAlongSpline % 1);
	}

    void FixedUpdate() {
        if (Vector3.Distance(transform.position, targetPoint) < 15) {
            targetAmountAlongSpline += 0.003f;
            targetPoint = centerLane.GetPoint(targetAmountAlongSpline % 1);

            Vector3 splineDirection = centerLane.GetVelocity(targetAmountAlongSpline % 1);
            Vector3 right = Vector3.Cross(Vector3.up, splineDirection).normalized;

            targetPoint += right * lane * laneWidth;
        }

		otherThing.transform.position = targetPoint;

        Vector3 relativePosition = transform.InverseTransformPoint(targetPoint);

        relativePosition.y = 0;

        float angle = Vector3.Angle(Vector3.forward, relativePosition);

        if (relativePosition.x < 0) {
            angle = -angle;
        };

        SetSteer(angle);
    }

    public void ChangeLaneRight() {
        if (++lane > numOfLanesWideFromCenter) {
            lane--;
        }
    }

    public void ChangeLaneLeft() {
        if (--lane < -numOfLanesWideFromCenter) {
            lane++;
        }   
    }

    public float GetLane() {
        return lane;
    }

    public void SteerToMiddle() {
        lane = 0;
    }

    public void SteerToLeft() {
		lane = -1;
    }

    public void SteerToRight() {
        lane = 1;
    }

    public float GetDistanceToNextCorner() {
        TrackArea currentArea = trackAreaManager.GetCurrentTrackArea(targetAmountAlongSpline % 1);

        if (currentArea != null) {
            return 0;
        }

        TrackArea trackArea = trackAreaManager.GetNextTrackArea(targetAmountAlongSpline % 1);
        centerLane.GetPoint(trackArea.start);

        return Vector3.Distance(car.transform.position, centerLane.GetPoint(trackArea.start));
    }

    public float GetNextCornerAmount() {

        TrackArea currentArea = trackAreaManager.GetCurrentTrackArea(targetAmountAlongSpline % 1);

        if (currentArea != null) {
            return currentArea.cornerAmount;
        }
		
        return trackAreaManager.GetNextTrackArea(targetAmountAlongSpline % 1).cornerAmount;
    }

    public float GetDistanceToCar(float deg) {
        Vector3 direction = Quaternion.AngleAxis(deg, transform.up) * transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(detector.position, direction, out hit, Mathf.Infinity, cars)) {
            return hit.distance;
        }

		return -1;
    }

    public bool CarInFront() {
        float distRight = GetDistanceToCar(30);

        float distLeft = GetDistanceToCar(-30);

        float dist = GetDistanceToCar(0);

        return (distRight > 0 && distRight < 15) || (distLeft > 0 && distLeft < 15) || (dist > 0 && dist < 15);
    }

    public bool CarOnRight() {
        float distFront = GetDistanceToCar(75);

        float distBack = GetDistanceToCar(105);

        return (distFront > 0 && distFront < 5) || (distBack > 0 && distBack < 5);
    }

    public bool CarOnLeft() {
        float distFront = GetDistanceToCar(-75);

        float distBack = GetDistanceToCar(-105);

        return (distFront > 0 && distFront < 5) || (distBack > 0 && distBack < 5);
    }

	public float GetSpeed() {
		return rigidbody.velocity.magnitude;
	}

	public void SetThrottle(float value) {
		value = Mathf.Clamp(value, -100, 100);
		car.SetThrottle(value / 100.0f);
	}

	public void SetBrake(float value) {
		value = Mathf.Clamp(value, 0, 100);
		car.SetThrottle(value);
	}

	public void SetSteer(float value) {
		value = Mathf.Clamp(value, -45, 45);
		car.SetSteer(value);
	}

    public void Boost() {
        car.Boost();
    }

    public float GetTimeToNextBoost() {
        return car.GetTimeToNextBoost();
    }
}
