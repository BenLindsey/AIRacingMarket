using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {

    public OurCar car;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        car.SetSteer(Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1) * 45);
        car.SetThrottle(Mathf.Clamp(Input.GetAxis("Vertical"), -1, 1));

		car.SetBrake((Input.GetButton("Brake") ? 1 : 0) * 100f);

        if (Input.GetButton("Reset")) {
            car.Reset();
        }
	}
}
