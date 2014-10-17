using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerControl : MonoBehaviour {
	
	public GameObject player;
	public HimControl him;
	public float startZ = 0;
	
	public float speed = 13f;
	public float turnSpeed = 200f;
	
	private bool inAction = false;
	public Vector3 target;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(!inAction && player.transform.localPosition.z > startZ) {
			inAction = true;
			GetComponent<TextManager>().startTime = Time.time;
			this.audio.Play();
		}
	}
	
	void FixedUpdate() {
		
		if(!inAction) {
			return;
		}
		
		Vector3 relativePos = target - transform.localPosition;
		
		Vector3 rotationToLocation = Vector3.Cross(transform.forward, relativePos);
		Vector3 rotationToUp = Vector3.Cross(transform.up, Vector3.up);
		
		rigidbody.AddTorque((rotationToLocation+rotationToUp)*turnSpeed);
		
		rigidbody.AddForce(transform.forward*speed);
	}
}
