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

		float frontDistance = api.GetDistanceToNearestObstacle(0);

		frontDistance = Mathf.Min(100, frontDistance);

		if (frontDistance < 0) {
			frontDistance = 100;
		}
		if (frontDistance < 38) {
			if (api.GetSpeed() > 7) { 
				api.SetThrottle(-50f);
			} else {
				api.SetThrottle (50f);
			}
		} else {
			api.SetThrottle(80f);
		}
		/*
		if (api.GetSpeed() > 10) {
			api.SetThrottle(0f);
		} else {
			api.SetThrottle(80f);
		}*/

		Debug.Log(api.GetSpeed());
	}
}
