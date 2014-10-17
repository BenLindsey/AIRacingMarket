using UnityEngine;
using System.Collections;

public class FirstAI : MonoBehaviour {

	AiApi api;
	
	void Start () {
		api = GetComponentInChildren<AiApi>();
		api.SetThrottle(45f);
	}

	void FixedUpdate () {

		float rightDistance = api.GetDistanceToNearestObstacle(75);
		float leftDistance = api.GetDistanceToNearestObstacle(-75);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

		api.SetSteer(position * 45);

		/*if (Mathf.Abs(position) > 0.9) {
			api.SetThrottle(100f);
		} else {
			api.SetThrottle(70f);
		}*/
	}
}
