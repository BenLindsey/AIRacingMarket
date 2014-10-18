using UnityEngine;
using System.Collections;

public class FirstAI : MonoBehaviour {

	AiApi api;
	
	void Start () {
		api = GetComponentInChildren<AiApi>();
		api.SetThrottle(15f);
	}

	void FixedUpdate () {

		float rightDistance = api.GetDistanceToNearestObstacle(75);
		float leftDistance = api.GetDistanceToNearestObstacle(-75);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

		api.SetSteer(position * 45);

		if (api.GetSpeed() > 10) {
			api.SetThrottle(-20f);
		} else {
			api.SetThrottle(70f);
		}
	}
}
