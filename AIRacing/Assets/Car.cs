using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(new Vector3(0,0,10));
	}
}
