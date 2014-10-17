using UnityEngine;
using System.Collections;

public class FirstAI : MonoBehaviour {

	AiApi api;
	
	void Start () {
		api = GetComponentInChildren<AiApi>();

		api.SetThrottle(50f);
	}

	void FixedUpdate () {
		/*
		if (Time.time > 2) {
			api.SetSteer(30);
		}*/

		float rightDistance = api.GetDistanceToNearestObstacle(90);
		float leftDistance = api.GetDistanceToNearestObstacle(-90);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		//rightDistance = Mathf.Clamp(rightDistance, 0, 50);
		//leftDistance = Mathf.Clamp(leftDistance, 0, 50);
		//rightDistance = (rightDistance + lastRight) / 2;
		//leftDistance = (leftDistance + lastLeft) / 2;

		float difference = rightDistance - leftDistance;

		api.SetSteer(difference * 15);
		//Debug.Log(difference * 10);
	}
}
