using UnityEngine;
using System.Collections;

public class FirstAI : MonoBehaviour {

	AiApi api;

	void Start () {
		api = GetComponentInChildren<AiApi>();
	}

	void FixedUpdate () {

		float rightDistance = api.GetDistanceToEdge(65);
		float leftDistance = api.GetDistanceToEdge(-65);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

		api.SetSteer(position * 35);

		float frontDistance = api.GetDistanceToEdge(0);

		frontDistance = Mathf.Min(100, frontDistance);

		if (frontDistance < 0) {
			frontDistance = 100;
		}

		if (frontDistance < 50) { 
			if (api.GetSpeed() < 15) { 
				api.SetThrottle(0f);
			} else {
				api.SetThrottle(-50f);
			}
		} else {
			api.SetThrottle(70f);
		}

		//Debug.Log(api.GetSpeed());
	}
}
