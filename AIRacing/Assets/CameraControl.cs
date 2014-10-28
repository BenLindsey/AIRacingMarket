using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private Camera[] cameras;

    private int activeCamera = 0;

	// Use this for initialization
	void Start () {
        cameras = GetComponentsInChildren<Camera>();

        for (int index = 0; index < cameras.Length; index++) {
            cameras[index].enabled = (index == activeCamera);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("CameraToggle")) {
            cameras[activeCamera].enabled = false;
            activeCamera = (activeCamera + 1) % cameras.Length;
            cameras[activeCamera].enabled = true;
        }
	}
}
