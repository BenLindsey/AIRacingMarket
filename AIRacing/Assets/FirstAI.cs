using UnityEngine;
using System.Collections;

public class FirstAI : MonoBehaviour {

	AiApi api;
	
	void Start () {
		api = GetComponentInChildren<AiApi>();

		api.SetThrottle(0.00001f);
	}

	void FixedUpdate () {

		float rightDistance = api.GetDistanceToNearestObstacle(90);
		float leftDistance = api.GetDistanceToNearestObstacle(-90);

		if (rightDistance < 0 || leftDistance < 0) {
			return;
		}

		rightDistance = Mathf.Clamp(rightDistance, 0, 10);
		leftDistance = Mathf.Clamp(leftDistance, 0, 10);

		float difference = rightDistance - leftDistance;

		api.SetSteer(difference);
		Debug.Log(difference);
	}
}
