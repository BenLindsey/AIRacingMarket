using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CarManager : MonoBehaviour {

	public Transform[] startPositions;

	private List<GameObject> cars = new List<GameObject>();

	private string folder = Directory.GetCurrentDirectory() + "\\Assets\\Racing Scripts\\";

	public GameObject original;

	private List<Camera> cameras = new List<Camera>();
	private int activeCamera = 0;
	
	// Use this for initialization
	void Start () {
		cameras.Add(original.GetComponentInChildren<Camera>());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("CameraToggle")) {
			cameras[activeCamera].enabled = false;
			activeCamera = (activeCamera + 1) % cameras.Count;
			cameras[activeCamera].enabled = true;
		}
	}

	public void AddLocalScript(string name) {
		if (cars.Count == startPositions.Length) {
			return;
		}

		GameObject newCar;

		if (cars.Count == 0) {
			newCar = original;
		} else {
			newCar = Instantiate(original) as GameObject;
			cameras.Add(newCar.GetComponentInChildren<Camera>());
		}

		newCar.transform.position = startPositions[cars.Count].position;
		newCar.transform.rotation = startPositions[cars.Count].rotation;

		cars.Add(newCar);

		string path = folder + name;
		if (File.Exists(path)) {
			Debug.Log("Setting local script path to: " + path);
			newCar.transform.FindChild("OurCar").SendMessage("SetLocalScriptPath", path);
		}
		else {
			Debug.Log("Path: '" + path + "' does not exist.");
		}
	}

	public void AddRemoteScript(string name) {
		
	}

	public void StartRace() {
		foreach (GameObject car in cars) {
			car.SetActive(true);
		}

		foreach (Transform t in startPositions) {
			t.gameObject.SetActive(false);
		}
	}
}
