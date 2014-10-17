using UnityEngine;
using System.Collections;

public class RoadCrosser : MonoBehaviour {
	
	public float DeathImpact = 100;
	
	private Vector3 startLocation;
	
	void Start() {
		startLocation = transform.localPosition;	
	}
	
	
	void OnCollisionEnter(Collision collision) {
		if(collision.impactForceSum.magnitude > DeathImpact) {
			transform.localPosition = startLocation;
			rigidbody.velocity = new Vector3(0,0,0);
		}
	}
	
	void OnTriggerStay(Collider other) {
		if(other.collider.name == "PeopleArea") {
			other.transform.parent.gameObject.GetComponent<CrossingControl>().crossing = true;
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(other.collider.name == "PeopleArea") {
			other.transform.parent.gameObject.GetComponent<CrossingControl>().crossing = false;
		}	
	}
}
