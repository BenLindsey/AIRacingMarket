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
    public List<GameObject> Cars { get { return cars; } }

	public GameObject original;

    public CameraManager cameraManager;

    private bool isRaceStarted = false;
    public bool IsRaceStarted { get { return isRaceStarted; } }

	// Use this for initialization
	void Start () {
        Application.ExternalCall("RequestGameInfo");
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AddScriptByName(string scriptName) {
        if (cars.Count == startPositions.Length) {
            return;
        }

        GameObject newCar = Instantiate(original) as GameObject;
        newCar.SetActive(true);
        cameraManager.AddCamera(newCar.GetComponentInChildren<Camera>());

        newCar.transform.position = startPositions[cars.Count].position;
        newCar.transform.rotation = startPositions[cars.Count].rotation;

        cars.Add(newCar);

        newCar.transform.FindChild("OurCar").SendMessage("SetCar", cars.Count);

        newCar.SetActive(false);
    }

    public void AddScriptContent(string scriptContent) {
        cars[cars.Count - 1].SetActive(true);
        Debug.Log("Trying to add script...: " + scriptContent);
        cars[cars.Count - 1].transform.FindChild("OurCar").SendMessage("SetScriptContent", scriptContent);
        cars[cars.Count - 1].SetActive(false);
    }

    public void SetCarModel(string carName) {
        Debug.Log("Setting car models...");
        CarModelSelector carModelSelector = new CarModelSelector();
        foreach (GameObject car in cars) {
            carModelSelector.createCar(carName, car);
        }
    }

    public void ExecuteCommands(int car, string commands) {
        cars[car].transform.FindChild("OurCar").SendMessage("ExecuteCommands", commands);   
    }

	public void StartRace(string arg) {
        Debug.Log("Race started with " + cars.Count + " racers!");

		foreach (GameObject car in cars) {
			car.SetActive(true);
		}

        foreach (Transform t in startPositions) {
            t.gameObject.SetActive(false);
        }

        isRaceStarted = true;
	}
}
