using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {

    private List<Camera> cameras = new List<Camera>();
    private int activeCamera = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("CameraToggle")) {
            cameras[activeCamera].enabled = false;
            activeCamera = (activeCamera + 1) % cameras.Count;
            cameras[activeCamera].enabled = true;
        }
	}

    public void AddCamera(Camera camera) {
        cameras.Add(camera);
    }

}

