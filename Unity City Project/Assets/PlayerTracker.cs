using UnityEngine;
using System.Collections;

public class PlayerTracker : MonoBehaviour {

	
	public Transform player;
	
	private Vector3 startLocation;
	
	// Use this for initialization
	void Start () {
		startLocation = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 newLocation = startLocation;
		newLocation.z = player.localPosition.z - 13;
		
		transform.localPosition = newLocation;
	}
}
