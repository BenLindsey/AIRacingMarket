using UnityEngine;
using System.Collections;

public class AiApi : MonoBehaviour {

	public LayerMask ignoreLayers;
	Transform detector;

	OurCar car;

	// Use this for initialization
	void Start () {
		car = GetComponent<OurCar>();
		detector = transform.FindChild("Detector");
	}

	public float GetDistanceToNearestObstacle(float deg) {

		Vector3 direction = Quaternion.AngleAxis(deg, transform.up) * transform.forward;

		RaycastHit hit;
		if (Physics.Raycast(detector.position, direction, out hit, ignoreLayers.value)) {
			return hit.distance;
		}

		return -1;
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
