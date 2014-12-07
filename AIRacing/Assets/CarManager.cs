using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

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

    private List<string> colours = new List<string>(){
        "red",
        "yellow",
        "orange",
        "green"
    };

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
        Debug.Log("Adding script: " + scriptName);
        if (cars.Count == startPositions.Length) {
            Debug.Log("Too many cars added. (Count is " + cars.Count + ", max is "
                + startPositions.Length + ")");
            return;
        }

        GameObject newCar = Instantiate(original) as GameObject;
        newCar.SetActive(true);
        cameraManager.AddCamera(newCar.GetComponentInChildren<Camera>());

        newCar.transform.position = startPositions[cars.Count].position;
        newCar.transform.rotation = startPositions[cars.Count].rotation;

        SetCarColour(newCar);

        cars.Add(newCar);

        newCar.transform.FindChild("OurCar").SendMessage("SetCar", cars.Count - 1);
        // Do not remove this line! The hud will be sad otherwise.
        newCar.transform.FindChild("OurCar").SendMessage("SetScriptName", scriptName);

        newCar.SetActive(false);
    }

    public void AddScriptContent(string scriptContent) {
        cars[cars.Count - 1].SetActive(true);
        Debug.Log("Trying to add script...: " + scriptContent);
        cars[cars.Count - 1].transform.FindChild("OurCar").SendMessage("SetScriptContent", scriptContent);
        cars[cars.Count - 1].SetActive(false);
    }

    private void SetCarColour(GameObject car) {

        System.Random random = new System.Random();
        int selected = random.Next(colours.Count);

        // Set a default colour (red) if we're using more than four cars.
        string colour = colours.Count == 0 ? "red" : colours[selected];

        Debug.Log("Setting car colour to " + colour);

        MeshRenderer carRenderer = car.transform.FindChild("OurCar/Body").GetComponent<MeshRenderer>();
        foreach (Material material in carRenderer.materials) {
            material.mainTexture = Resources.Load<Texture2D>("Colours/CAR_body_" + colour);
        }

        colours.RemoveAt(selected);
    }

    public void SetCarModel(string carName) {
        Debug.Log("Setting car models...");
        CarModelSelector carModelSelector = new CarModelSelector();
        foreach (GameObject car in cars) {
            carModelSelector.createCar(carName, car);
        }
    }

    public void ExecuteCommands(string commandJSON) {
        //Debug.Log("Received execute command request");

        JSONNode data = JSON.Parse(commandJSON);

        //Debug.Log("Car: " + data["car"]);
        //Debug.Log("Instructions: " + data["instructions"]);

        cars[data["car"].AsInt].transform.FindChild("OurCar").SendMessage("ExecuteCommands", data["instructions"].Value);   
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
