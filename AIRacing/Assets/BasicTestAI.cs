using UnityEngine;
using System.Collections;

public class BasicTestAI : MonoBehaviour {

	AiApi api;
	
	void Start () {
		api = GetComponentInChildren<AiApi>();
	}

	void FixedUpdate () {

		float rightDistance = api.GetDistanceToNearestObstacle(65);
		float leftDistance = api.GetDistanceToNearestObstacle(-65);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		float position = 2 * (rightDistance - leftDistance) / (rightDistance + leftDistance);

		api.SetSteer(position * 40);


		float frontDistance = api.GetDistanceToNearestObstacle(0);
		
		frontDistance = Mathf.Min(100, frontDistance);
		
		if (frontDistance < 20) {
			if (api.GetSpeed() > 10) {
				api.SetThrottle(-60f);
			} else {
				api.SetThrottle(70f);
			}
		} else {
			api.SetThrottle(100f);
		}

		//Debug.Log(api.GetSpeed());
	}
}
