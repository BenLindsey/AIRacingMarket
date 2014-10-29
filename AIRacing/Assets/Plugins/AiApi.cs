using UnityEngine;
using System.Collections;

public class AiApi : MonoBehaviour {

    public LayerMask walls;
    public LayerMask cars;

    public BezierSpline middleLane;
	Transform detector;

	OurCar car;

	// Use this for initialization
	void Start () {
		car = GetComponent<OurCar>();
		detector = transform.FindChild("Detector");
	}

    public void SteerToMiddle() {
        float rightDistance = GetDistanceToEdge(40);
        float leftDistance = GetDistanceToEdge(-40);

        if (rightDistance < 0 || leftDistance < 0) {
            return;
        }

        float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

        SetSteer(position * 25f);
    }

    public void SteerToLeft() {
        float rightDistance = GetDistanceToEdge(40);
        float leftDistance = GetDistanceToEdge(-40);

        if (rightDistance < 0 || leftDistance < 0) {
            return;
        }

        float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

        SetSteer((position - 0.8f) * 25f);
    }

    public void SteerToRight() {
        float rightDistance = GetDistanceToEdge(40);
        float leftDistance = GetDistanceToEdge(-40);

        if (rightDistance < 0 || leftDistance < 0) {
            return;
        }

        float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

        SetSteer((position + 0.8f) * 25f);
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

	public void SetSteer(float value) {
		value = Mathf.Clamp(value, -45, 45);
		car.SetSteer(value);
	}
}
