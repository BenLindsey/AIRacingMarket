using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class HUD : MonoBehaviour {

    private struct CarState {
        public OurCar car;
        public int lap;
        public int stage;
        public string name;
        public float flipStartTime;
    }

    public CarManager carManager;

    private CarState[] carStates;

    public GameObject checkpointsContainer;
    private GameObject[] checkpoints;

    private GUIStyle style = new GUIStyle();

    private const int LAPS_IN_RACE = 1;
    private const int MAX_FLIP_TIME = 3;
    private EndOfRaceObject endOfRaceObject;

	// Use this for initialization
	public void Start () {

        checkpoints = LoadCheckpoints();

        style.fontSize = 15;
        style.normal.textColor = Color.yellow;
	}

    public void Update() {

        // Setup the car states as soon as the race starts.
        if (carStates == null && carManager.IsRaceStarted) {

            carStates = new CarState[carManager.Cars.Count];
            Dictionary<string, int> nameFrequencies = new Dictionary<string, int>();
            endOfRaceObject = new EndOfRaceObject(this, carStates.Length);

            for (int i = 0; i < carStates.Length; i++) {

                OurCar ourCar = carManager.Cars[i].GetComponentInChildren<OurCar>();
                carStates[i].car = ourCar;
                carStates[i].lap = 0;
                carStates[i].stage = 0;
                carStates[i].flipStartTime = -1;

                // If there are multiple cars with the same name, then add the
                // occurence number onto the end of the name.
                int nameFrequency = nameFrequencies.ContainsKey(ourCar.Name)
                    ? nameFrequencies[ourCar.Name] : 0;
                carStates[i].name = ourCar.Name + ((nameFrequency == 0)
                    ? "" : " " + (nameFrequency + 1));
                nameFrequencies[ourCar.Name] = nameFrequency + 1;

                // TESTING ONLY: End the race as soon as all cars are loaded.
                endOfRaceObject.Finish(carStates[i].name);
            }
        }

        // Check if any cars are have flipped.
        if (carStates != null) {
            for (int i = 0; i < carStates.Length; i++) {
                // Remove the car if it is flipped over for too long.
                if (carStates[i].car.IsFlipped) {
                    if (carStates[i].flipStartTime == -1) {
                        carStates[i].flipStartTime = Time.time;
                    } else if (Time.time - carStates[i].flipStartTime >= MAX_FLIP_TIME) {
                        endOfRaceObject.RemoveFromRace(carStates[i].name);
                    }
                }
                
                // Reset the timeout otherwise.
                else if (carStates[i].flipStartTime != -1) {
                    carStates[i].flipStartTime = -1;
                }
            }
        }
    }

    public void OnGUI() {

        CarState currentState = carStates[carManager.cameraManager.ActiveCamera];

        int y = 10;
        GUI.Label(new Rect(10, y, 200, 100), "Lap " + (currentState.lap + 1), style);
        y += 20;

        // Find the active camera to convert 3D coordinates to screen coordinates.
        Camera carCamera = currentState.car.transform.parent.GetComponentInChildren<Camera>();

        for (int i = 0; i < carStates.Length; i++) {
            int position = GetPosition(carStates[i]);

            // Draw the car's row on the leaderboard.
            GUI.Label(new Rect(10, y + 20 * position, 200, 100),
                position + ": " + carStates[i].name, style);

            // Draw the car's name over its model if it's in front of the camera.
            Vector3 screenPosition = carCamera.WorldToScreenPoint(
                carStates[i].car.transform.position + 2.5f * Vector3.up);
            if (screenPosition.z > 0) {
                float xOffset = style.CalcSize(new GUIContent(carStates[i].name)).x / 2;
                GUI.Label(new Rect(screenPosition.x - xOffset, carCamera.pixelHeight - screenPosition.y,
                    200, 100), carStates[i].name, style);
            }
        }
        y += 20 * carStates.Length;
    }

    public void TriggerEnter(int checkpoint, Collider car) {

        //int checkpointIndex = GetCheckpointIndexFromGameObject(checkpoint);
        int carIndex = GetCarIndexFromCollider(car);

        // Something bad happened, and a warning has already been logged, so just return.
        if (checkpoint == -1 || carIndex == -1) {
            return;
        }

        // If the car just completed the next stage of the lap.
        if (carStates[carIndex].stage == checkpoint) {
            carStates[carIndex].stage++;

            // If the car just completed a lap.
            if (carStates[carIndex].stage == checkpoints.Length) {
                carStates[carIndex].stage = 0;
                carStates[carIndex].lap++;

                // Update the WWW form if this car just finished the race.
                if (carStates[carIndex].lap == LAPS_IN_RACE) {
                    endOfRaceObject.Finish(carStates[carIndex].name);
                }
            }
        }

        // Otherwise, the car went through the wrong checkpoint. This is not
        // logged because each car has multiple colliders which trigger the checkpoints.
    }

    private int GetCarIndexFromCollider(Collider collider) {
        for (int i = 0; i < carStates.Length; i++) {
            if (collider.transform.IsChildOf(carStates[i].car.transform)) {
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
            if (other.car != car.car && !IsCarInFront(car, other)) {
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
        return Vector3.Distance(car.car.transform.position,
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

    private class EndOfRaceObject {
        private MonoBehaviour outerClass;

        private int carsRacing;
        private Queue<string> carsFinished = new Queue<string>();
        private Stack<string> carsRemoved = new Stack<string>();

        private HashSet<string> carsSeen = new HashSet<string>();

        public EndOfRaceObject(MonoBehaviour outerClass, int carCount) {
            this.outerClass = outerClass;
            carsRacing = carCount;
        }

        public void Finish(string scriptName) {
            if (carsRacing > 0 && !carsSeen.Contains(scriptName)) {
                carsFinished.Enqueue(scriptName);
                carsSeen.Add(scriptName);
                carsRacing--;

                Debug.Log(scriptName + " has finished the race!");

                if (carsRacing == 0) {
                    Send();
                }
            }
        }

        public void RemoveFromRace(string scriptName) {
            if (carsRacing > 0 && !carsSeen.Contains(scriptName)) {
                carsRemoved.Push(scriptName);
                carsSeen.Add(scriptName);
                carsRacing--;

                Debug.Log(scriptName + " was removed from the race!");

                if (carsRacing == 0) {
                    Send();
                }
            }
        }

        private void Send() {
            WWWForm form = new WWWForm();
            string[] fieldNames = new string[] { "first", "second", "third", "fourth" };
            int index = 0;

            Debug.Log("Race has finished!");
            while (carsFinished.Count > 0) {
                Debug.Log("Position " + index + ": " + carsFinished.Peek());

                form.AddField(fieldNames[index], carsFinished.Dequeue());
                index++;
            }

            while (carsRemoved.Count > 0) {
                Debug.Log("Position " + index + ": " + carsRemoved.Peek());

                form.AddField(fieldNames[index], carsRemoved.Pop());
                index++;
            }

            outerClass.StartCoroutine(WaitForSend(form));
        }

        private IEnumerator WaitForSend(WWWForm form) {
            int port = 3026;
            string url = "http://146.169.47.15:" + port + "/score/";

            Debug.Log("Sending data to '" + url + "' ...");
            WWW www = new WWW(url, form);

            yield return www;

            if (www.error == null) {
                Debug.Log("End of race object sent! " + www.text);
            }
            else {
                Debug.Log("Error sending: " + www.error);
            }
        }
    }
}
