using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour {
	
	public float speed = 100f;
	public float maxSpeed = 20f;
	public float steeringSpeed = 100f;
	private float defaultMaxSpeed;
	
	public Vector3[] points;
	public GameObject[] crossings;
	
	private int nextLocation = 0;
	public Vector3 start;
	
	private CrossingControl crossing = null;
	private TrafficLightControl trafficLight = null;
	
	// Use this for initialization
	void Start () {
		defaultMaxSpeed = maxSpeed;
	}
	
	void Update() {
		Vector3 front = transform.position;
		front += 5*transform.forward;
		
		if(Physics.Raycast(front, transform.forward,1f)) {
			maxSpeed = 0;
		} else if (Physics.Raycast(front, transform.forward,4f)) {
			maxSpeed = defaultMaxSpeed/5;
		} else {
			maxSpeed = defaultMaxSpeed;
		}
		
		if(crossing != null && crossing.crossing)	{
			maxSpeed = 0;
		}
		
		if(trafficLight != null && !trafficLight.green) {
			maxSpeed = 0;	
		}
	}
	
	void FixedUpdate() {
		
		Vector3 relativePos = points[nextLocation] - transform.localPosition;
		
		Vector3 rotation = Vector3.Cross(transform.forward, relativePos);
		
		rigidbody.AddTorque(rotation*steeringSpeed);
		
		if(relativePos.magnitude < 3){
			transform.localPosition= start;
		}
		
		if(rigidbody.velocity.magnitude < maxSpeed && Physics.Raycast(transform.localPosition,-transform.up,1.5f)){
			rigidbody.AddForce(transform.forward*speed);
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.collider.name == "CarArea") {
			crossing = other.transform.parent.gameObject.GetComponentInChildren<CrossingControl>();
		} 
		
		if(other.collider.name == "LightArea") {
			trafficLight = other.transform.parent.gameObject.GetComponentInChildren<TrafficLightControl>();
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(other.collider.name == "CarArea") {
			crossing = null;
		} 
		
		if(other.collider.name == "LightArea") {
			trafficLight = null;
		}
	}
}
