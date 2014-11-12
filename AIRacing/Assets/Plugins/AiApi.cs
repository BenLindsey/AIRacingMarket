using UnityEngine;
using System.Collections;

public class AiApi : MonoBehaviour {

    public LayerMask walls;
    public LayerMask cars;

    public BezierSpline[] lanes;

    int lane = 1;
    float targetAmountAlongSpline = 0.01f;
    Vector3 targetPoint;

	Transform detector;

	OurCar car;

    public GameObject debugOb;

	// Use this for initialization
	void Start () {
		car = GetComponent<OurCar>();
		detector = transform.FindChild("Detector");
        targetPoint = lanes[lane].GetPoint(targetAmountAlongSpline % 1);
	}

    void FixedUpdate() {
        if (Vector3.Distance(transform.position, targetPoint) < 15) {
            targetAmountAlongSpline += 0.001f;
            targetPoint = lanes[lane].GetPoint(targetAmountAlongSpline % 1);
            debugOb.transform.position = targetPoint;
        }

        Vector3 relativePosition = transform.InverseTransformPoint(targetPoint);

        relativePosition.y = 0;

        float angle = Vector3.Angle(Vector3.forward, relativePosition);

        if (relativePosition.x < 0) {
            angle = -angle;
        };

        SetSteer(angle);
    }

    public void SteerToMiddle() {
        lane = 1;

        while (Vector3.Distance(transform.position, targetPoint) < 15) {
            targetAmountAlongSpline += 0.001f;
            targetPoint = lanes[lane].GetPoint(targetAmountAlongSpline % 1);
            debugOb.transform.position = targetPoint;
        }
    }

    public void SteerToLeft() {
		lane = 0;

        while (Vector3.Distance(transform.position, targetPoint) < 15) {
            targetAmountAlongSpline += 0.001f;
            targetPoint = lanes[lane].GetPoint(targetAmountAlongSpline % 1);
            debugOb.transform.position = targetPoint;
        }
    }

    public void SteerToRight() {
        lane = 2;

        while (Vector3.Distance(transform.position, targetPoint) < 15) {
            targetAmountAlongSpline += 0.001f;
            targetPoint = lanes[lane].GetPoint(targetAmountAlongSpline % 1);
            debugOb.transform.position = targetPoint;
        }
    }

	public float GetDistanceToEdge(float deg) {

		Vector3 direction = Quaternion.AngleAxis(deg, transform.up) * transform.forward;

		RaycastHit hit;
        if (Physics.Raycast(detector.position, direction, out hit, Mathf.Infinity, walls)) {
			return hit.distance;
		}

		return -1;
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

        return (distFront > 0 && distFront < 15) || (distBack > 0 && distBack < 15);
    }

    public bool CarOnLeft() {
        float distFront = GetDistanceToCar(-75);

        float distBack = GetDistanceToCar(-105);

        return (distFront > 0 && distFront < 15) || (distBack > 0 && distBack < 15);
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
}
