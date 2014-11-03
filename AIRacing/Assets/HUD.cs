using UnityEngine;
using System;
using System.Collections.Generic;

public class HUD : MonoBehaviour {

    private struct CarState {
        public GameObject carObject;
        public int lap;
        public int stage;
    }

    public CarManager carManager;

    private CarState[] carStates;
    private int checkpointCount = 4; // TODO: Magic number.

    public GameObject checkpointsContainer;
    //private GameObject[] checkpoints;

	// Use this for initialization
	public void Start () {

        //checkpoints = LoadCheckpoints();
	}

    public void Update() {

        // Setup the car states as soon as the race starts.
        if (carStates == null && carManager.IsRaceStarted) {

            carStates = new CarState[carManager.Cars.Count];
            for (int i = 0; i < carStates.Length; i++) {
                carStates[i].carObject = carManager.Cars[i];
                carStates[i].lap = 0;
                carStates[i].stage = 0;
            }
        }
    }

    public void OnGUI() {

        CarState currentState = carStates[carManager.cameraManager.ActiveCamera];

        GUI.Label(new Rect(10, 10, 100, 100), "Lap " + (currentState.lap + 1));
    }

    public void TriggerEnter(int checkpoint, Collider car) {

        //int checkpointIndex = GetCheckpointIndexFromGameObject(checkpoint);
        int checkpointIndex = checkpoint;
        int carIndex = GetCarIndexFromCollider(car);

        // Something bad happened, and a warning has already been logged, so just return.
        if (checkpointIndex == -1 || carIndex == -1) {
            return;
        }

        // If the car just completed the next stage of the lap.
        if (carStates[carIndex].stage == checkpointIndex) {
            carStates[carIndex].stage++;

            Debug.Log("Car " + carIndex + " completed stage " + checkpointIndex);
        }

        // If the car just completed a lap.
        else if (carStates[carIndex].stage == checkpointCount && checkpointIndex == 0) {
            carStates[carIndex].stage = 0;
            carStates[carIndex].lap++;

            Debug.Log("Car " + carIndex + " completed lap " + carStates[carIndex].lap);
        }

        // Otherwise, the car went through the wrong checkpoint. This is not
        // logged because each car has multiple colliders which trigger the checkpoints.
    }

    private int GetCarIndexFromCollider(Collider collider) {

        for (int i = 0; i < carStates.Length; i++) {
            if (collider.transform.IsChildOf(carStates[i].carObject.transform)) {
                return i;
            }
        }

        Debug.LogWarning("An object which is not a car belonging to the CarManager "
            + "passed through a checkpoint.");
        return -1;
    }

    //private int GetCheckpointIndexFromGameObject(GameObject checkpoint) {

    //    for (int i = 0; i < checkpoints.Length; i++) {
    //        if (checkpoint == checkpoints[i]) {
    //            return i;
    //        }
    //    }

    //    Debug.LogWarning("The checkpoint " + checkpoint + " is not a child of "
    //        + checkpointsContainer.name);
    //    return -1;
    //}

    //private GameObject[] LoadCheckpoints() {

    //    List<GameObject> children = new List<GameObject>();
    //    for (int i = 0; i < checkpointsContainer.transform.GetChildCount(); i++) {

    //        Transform child = checkpointsContainer.transform.GetChild(i);
    //        BoxCollider collider = child.GetComponent<BoxCollider>();

    //        if (collider != null && collider.isTrigger) {
    //            children.Add(child.gameObject);
    //            Debug.Log("Found checkpoint " + child);
    //        }
    //        else {
    //            Debug.LogWarning("Child " + i + " of " + checkpointsContainer.name
    //                + " must have a trigger box collider to be a checkpoint.");
    //        }
    //    }

    //    return children.ToArray();
    //}
}
