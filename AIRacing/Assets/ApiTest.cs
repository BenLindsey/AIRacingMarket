using UnityEngine;
using System.Collections;

public class ApiTest : MonoBehaviour {

    public AiApi api;

	// Use this for initialization
	void Start () {
        api.SteerToLeft();
	}
	
	// Update is called once per frame
	void Update () {
        float distance = api.GetDistanceToNextCorner();

        if (distance > 100) {
            api.Boost();
        }

        float maxSpeed = (distance / 8) + 20;

        Debug.Log(maxSpeed);

        if (api.GetSpeed() < maxSpeed) {
            api.SetThrottle(70f);
        }
        else {
            api.SetThrottle(0);
        }
	}
}
