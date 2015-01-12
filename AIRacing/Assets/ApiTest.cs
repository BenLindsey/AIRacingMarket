using UnityEngine;
using System.Collections;

public class ApiTest : MonoBehaviour {

    public AiApi api;

	// Use this for initialization
	void Start () {
        api.SteerToMiddle();
		api.Boost();
	}
	
	// Update is called once per frame
	void Update () {

        float maxSpeed = 23;
	

        if (api.GetSpeed() < maxSpeed) {
            api.SetThrottle(70f);
        }
        else {
            api.SetThrottle(0);
        }
	}
}
