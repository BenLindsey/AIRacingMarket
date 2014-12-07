using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {

	public GameObject audioListener;

    private List<Camera> cameras = new List<Camera>();

    private int activeCamera = 0;
    public int ActiveCamera { get { return activeCamera; } }

    private bool automaticCamera = false;
    private float cameraTimer = 0f;
    private const float CAMERA_TOGGLE_RATE = 4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        cameraTimer += Time.deltaTime;
	    if (Input.GetButtonDown("CameraToggle") || (automaticCamera && cameraTimer > CAMERA_TOGGLE_RATE)) {

            // Reset timer.
            cameraTimer = 0f;

			cameras[activeCamera].enabled = false;

            // Activate the new camera 
            activeCamera = (activeCamera + 1) % cameras.Count;
            cameras[activeCamera].enabled = true;
        }

		audioListener.transform.position = cameras[activeCamera].transform.position;
	}

    public void AddCamera(Camera camera) {
		if (cameras.Count != activeCamera) {
			camera.enabled = false;
		}

        cameras.Add(camera);
    }

    public void SetAutomaticCamera() {
        automaticCamera = true;
    }
}

