using UnityEngine;
using System.Collections;

public class PersonControl : MonoBehaviour {

	public float speed = 13f;
	public float turnSpeed = 200f;
	
	public Vector3[] points;
	
	private int nextLocation = 0;
	
	public Vector3 endPoint;
	
	// Use this for initialization
	void Start () {
	}
	
	void FixedUpdate() {
		
		Vector3 relativePos = points[nextLocation] - transform.localPosition;
		
		Vector3 rotationToLocation = Vector3.Cross(transform.forward, relativePos);
		Vector3 rotationToUp = Vector3.Cross(transform.up, Vector3.up);
		
		rigidbody.AddTorque((rotationToLocation+rotationToUp)*turnSpeed);
		
		if(relativePos.magnitude < 5) {
			nextLocation = (nextLocation + 1) % points.Length;
		}
		
		if((endPoint - transform.localPosition).magnitude < 2){
			nextLocation = 0;
			transform.localPosition = points[nextLocation];
		}
		
		rigidbody.AddForce(transform.forward*speed);
	}
}

