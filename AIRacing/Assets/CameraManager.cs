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

            // Disable the current camera and destroy its audio listener. Unity
            // will complain about multiple audio listeners even if only one is
            // enabled.
            cameras[activeCamera].enabled = false;
            Destroy(cameras[activeCamera].gameObject.GetComponent<AudioListener>());

            // Activate the new camera and create an audio listener 
            activeCamera = (activeCamera + 1) % cameras.Count;
            cameras[activeCamera].enabled = true;
            cameras[activeCamera].gameObject.AddComponent<AudioListener>();
        }
	}

    public void AddCamera(Camera camera) {
        cameras.Add(camera);

        for (int i = 0; i < cameras.Count; i++) {
            cameras[i].enabled = (i == activeCamera);
            if (i == activeCamera) {
                cameras[activeCamera].gameObject.AddComponent<AudioListener>();
            }
        }
    }
}

