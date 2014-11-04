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

    public GameObject checkpointsContainer;
    private GameObject[] checkpoints;

    private GUIStyle style = new GUIStyle();

	// Use this for initialization
	public void Start () {

        checkpoints = LoadCheckpoints();

        style.fontSize = 15;
	}

    public void Update() {

        // Setup the car states as soon as the race starts.
        if (carStates == null && carManager.IsRaceStarted) {

            carStates = new CarState[carManager.Cars.Count];
            for (int i = 0; i < carStates.Length; i++) {
                carStates[i].carObject = carManager.Cars[i].GetComponentInChildren<OurCar>().gameObject;
                carStates[i].lap = 0;
                carStates[i].stage = 0;
            }
        }
    }

    public void OnGUI() {

        CarState currentState = carStates[carManager.cameraManager.ActiveCamera];

        int y = 10;
        GUI.Label(new Rect(10, y, 200, 100), "Lap " + (currentState.lap + 1), style);
        GUI.Label(new Rect(10, y += 20, 200, 100), "Position " + GetPosition(currentState)
            + " / " + checkpoints.Length, style);
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
            carStates[carIndex].stage = (carStates[carIndex].stage + 1) % checkpoints.Length;

            // If the car just completed a lap.
            if (carStates[carIndex].stage == 0) {
                carStates[carIndex].lap++;
            }
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

    private int GetPosition(CarState car) {
        int position = 1;
        foreach (CarState other in carStates) {
            // Increment the position each time we find a car which is in front
            // of the given car.
            if (other.carObject != car.carObject && !IsCarInFront(car, other)) {
                position++;
            }
        }

        return position;
    }

    private bool IsCarInFront(CarState car, CarState other) {
        return (car.lap > other.lap)
            || (car.lap == other.lap && car.stage > other.stage)
            || (car.lap == other.lap && car.stage == other.stage
                && DistanceToCheckpoint(car) < DistanceToCheckpoint(other));
    }

    private float DistanceToCheckpoint(CarState car) {
        return Vector3.Distance(car.carObject.transform.position,
            checkpoints[car.stage].transform.position);
    }

    private GameObject[] LoadCheckpoints() {

        GameObject[] children = new GameObject[checkpointsContainer.transform.childCount];
        for (int i = 0; i < children.Length; i++) {

            Transform child = checkpointsContainer.transform.GetChild(i);
            BoxCollider collider = child.GetComponent<BoxCollider>();
            Checkpoint checkpoint = collider.GetComponent<Checkpoint>(); // TODO: Fix case when collider is null.

            if (collider != null && collider.isTrigger && checkpoint != null) {
                children[checkpoint.checkpointNumber] = child.gameObject;
                Debug.Log("Found checkpoint " + child);
            } else {
                Debug.LogWarning("Child " + i + " of " + checkpointsContainer.name
                    + " must have a trigger box collider to be a checkpoint.");
            }
        }

        return children;
    }
}
