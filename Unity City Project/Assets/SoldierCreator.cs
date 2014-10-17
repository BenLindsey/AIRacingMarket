using UnityEngine;
using System.Collections;

public class SoldierCreator : MonoBehaviour {

	public float speed = 100f;
	
	void Start () {
	
	}
	
	void FixedUpdate() {
		Vector3 direction = new Vector3();
		
		if(Input.GetKey(KeyCode.W))
		{
			direction += Vector3.forward;
		} 
		
		if(Input.GetKey(KeyCode.S))
		{
			direction += Vector3.back;
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			direction += Vector3.right;
		}
		
		if(Input.GetKey(KeyCode.A))
		{
			direction += Vector3.left;
		}
		
		rigidbody.AddForce(direction.normalized*speed);
	}
}
