using UnityEngine;
using System.Collections;

public class Thought : MonoBehaviour {

	public float time;
	
	private float startTime;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(time < 0) {
			return;	
		}
		
		if(Time.time > time + startTime) {
			Destroy(this.gameObject);	
		}
	}
}
