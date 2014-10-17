using UnityEngine;
using System.Collections;

public class TrafficLightControl : MonoBehaviour {
	
	public bool green = false;
	public bool slave = false;
	public bool inverseSlave = false;
	public TrafficLightControl master;
	public float interval;
	public float crossOverTime;
	
	public Material redMat, greenMat;
	
	private float timeSinceSwitch = 0;
	public bool crossOver = true;
	private bool nextValue = true;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(slave) {
			if(master.crossOver) {
				green = false;	
			} else if(inverseSlave) {
				green = !master.green;
			} else {
				green = master.green;
			}
		} else {
			
			timeSinceSwitch += Time.deltaTime;
			
			if(crossOver) {
				if(timeSinceSwitch > crossOverTime) {
					timeSinceSwitch = 0;
					crossOver = false;
					green = nextValue;
				}
			} else {
				if(timeSinceSwitch > interval) {
					nextValue = !green;
					green = false;
					timeSinceSwitch = 0;
					crossOver = true;
				}
			}
		}
		
		if(green) {
			GetComponent<MeshRenderer>().material = greenMat;	
		} else {
			GetComponent<MeshRenderer>().material = redMat;	
		}
	}
}
