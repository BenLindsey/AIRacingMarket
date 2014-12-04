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

        float maxSpeed = (distance / 7) + 20;

        Debug.Log(maxSpeed);

        if (api.GetSpeed() < maxSpeed) {
            api.SetThrottle(70f);
        }
        else {
            api.SetThrottle(0);
        }
	}
}
