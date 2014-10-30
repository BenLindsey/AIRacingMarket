using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct Script
{
    public string name;
    public string contents;
}

public class CarManager : MonoBehaviour {

    public static string URL = "http://146.169.47.15:3001/";

	public Transform[] startPositions;

	private List<GameObject> cars = new List<GameObject>();

	public GameObject original;

    private CameraManager cameraManager;

	// Use this for initialization
	void Start () {
        cameraManager = GetComponent<CameraManager>();
        cameraManager.AddCamera(original.GetComponentInChildren<Camera>());
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AddScript(Script script) {
        if (cars.Count == startPositions.Length) {
            return;
        }

        GameObject newCar;

        if (cars.Count == 0) {
            newCar = original;
        }
        else {
            newCar = Instantiate(original) as GameObject;
            cameraManager.AddCamera(newCar.GetComponentInChildren<Camera>());
            newCar.GetComponentInChildren<Camera>().enabled = false;
        }

        newCar.transform.position = startPositions[cars.Count].position;
        newCar.transform.rotation = startPositions[cars.Count].rotation;

        cars.Add(newCar);

        newCar.transform.FindChild("OurCar").SendMessage("SetScriptName", script.name);
        newCar.transform.FindChild("OurCar").SendMessage("SetScriptName", script.contents);
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
