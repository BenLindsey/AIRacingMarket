using UnityEngine;
using System.Collections;

public class AiApi : MonoBehaviour {

	public Car car;

	// Use this for initialization
	void Start () {
	}

	public float GetDistanceToNearestObstacle(float deg) {

		Vector3 direction = Quaternion.AngleAxis(deg, transform.up) * transform.forward;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, direction, out hit)) {
			return hit.distance;
		}

		return -1;
	}

	public void SetThrottle(float value) {
		value = Mathf.Clamp(value, -100, 100);
		car.SetThrottle(value / 100.0f);
	}

	public void SetSteer(float value) {
		value = Mathf.Clamp(value, -100, 100);
		car.SetSteer(value / 100f);
	}
}
