using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour  {

    float power = 100f;
    float throttle;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidbody.AddForce(transform.forward * power * throttle);
	}

    void OnCollisionStay(Collision collisionInfo) {

    }

    public void SetThrottle(float amount) {
        throttle = amount;
    }
}
