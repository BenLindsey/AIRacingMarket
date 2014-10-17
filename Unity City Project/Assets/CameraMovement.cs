using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float speed = 10f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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
		
		transform.Translate(direction.normalized*Time.deltaTime*speed);
	}
}
