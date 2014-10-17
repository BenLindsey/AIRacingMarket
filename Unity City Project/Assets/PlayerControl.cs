using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public float speed = 10;
	public float maxSpeed = 100;
	public float turnSpeed = 100;
	
	public HerControl her;
	
	// Use this for initialization
	void Start () {
	}
	
	void Update() {
		/*if(transform.localPosition.z > her.startZ + 5) {
			maxSpeed = 3;	
		}*/
	}
	
	void FixedUpdate () {
		
		Vector3 rotationToUp = Vector3.Cross(transform.up, Vector3.up);
		rigidbody.AddTorque(rotationToUp*turnSpeed);
		
		Vector3 direction = new Vector3();
		
		if(Input.GetKey(KeyCode.W))
		{
			direction.z = 1;
		} 
		
		if(Input.GetKey(KeyCode.S))
		{
			direction.z = -1;
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			direction.x = 1;
		}
		
		if(Input.GetKey(KeyCode.A))
		{
			direction.x = -1;
		}
		
		if(/*Time.time > 6 && */rigidbody.velocity.magnitude < maxSpeed) {
			rigidbody.AddForce(direction*speed);
		}
	}
}
