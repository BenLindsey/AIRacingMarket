using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

public class HUD : MonoBehaviour {

    private struct CarState {
        public OurCar car;
        public int position;
        public int lap;
        public int stage;
        public string name;

        public float flipStartTime;
        private float lastCheckpointTime;

        public bool HasCarStopped { get { return Time.time - lastCheckpointTime >= MAX_CHECKPOINT_TIME; } }

        public void ResetCheckpointTime() {
            lastCheckpointTime = Time.time;
        }
    }

    private enum RaceMode { Multiplayer, Test, Tournament };

    public CarManager carManager;

    private CarState[] carStates;
    private int carsFinished = 0;

    public GameObject checkpointsContainer;
    private GameObject[] checkpoints;

    private GUIStyle style;
    private GUIStyle ordinalStyle;

    private RaceMode raceMode = RaceMode.Test;

    private const int LAPS_IN_RACE = 3;
    private const int MAX_FLIP_TIME = 3;
    private const int MAX_CHECKPOINT_TIME = 30; // Number of seconds to complete a checkpoint.
    private EndOfRaceObject endOfRaceObject;

    #region Browser Interface

    // Endless race, no data is sent to the leaderboard.
    public void SetMultiplayerMode() {
        Debug.Log("SetMultiplayerMode called");

        SetMode(RaceMode.Multiplayer);
    }

    // Normal race but user is redirected to /tournament/next.
    public void SetTournamentMode() {
        Debug.Log("SetTournamentMode called");

        SetMode(RaceMode.Tournament);
        if (raceMode == RaceMode.Tournament) {
            carManager.cameraManager.SetAutomaticCamera();
        }
    }

    private void SetMode(RaceMode mode) {
        if (raceMode != RaceMode.Test) {
            Debug.LogWarning("Mode has already been changed to " + raceMode
                + " this call will be ignored.");
        // TODO: Check if this will always fail.
        //} else if (carManager.IsRaceStarted) {
        //    Debug.LogWarning("The call is too late! The race has already started.");
        } else {
            raceMode = mode;
        }
    }

    // Move all cars to the starting positions.
    // Can only be called in test mode.
    public void ResetRace() {
        Debug.Log("ResetRace called");

        if (raceMode != RaceMode.Test) {
            Debug.LogWarning("The race can only be reset in Test mode.");
        } else {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
    #endregion

	// Use this for initialization
	public void Start () {
        checkpoints = LoadCheckpoints();

        style = new GUIStyle();
        style.fontSize = 23;
        style.normal.textColor = Color.white;

        ordinalStyle = new GUIStyle(style);
        ordinalStyle.fontSize = 13;
	}

    public void Update() {
        // Setup the car states as soon as the race starts.
        if (carStates == null && carManager.IsRaceStarted) {

            carStates = new CarState[carManager.Cars.Count];

            Dictionary<string, int> nameFrequencies = new Dictionary<string, int>();
            endOfRaceObject = new EndOfRaceObject(this);

            for (int i = 0; i < carStates.Length; i++) {
                OurCar ourCar = carManager.Cars[i].GetComponentInChildren<OurCar>();
                carStates[i].car = ourCar;
                carStates[i].position = i;
                carStates[i].lap = 0;
                carStates[i].stage = 0;
                carStates[i].flipStartTime = -1;
                carStates[i].ResetCheckpointTime();

                // If there are multiple cars with the same name, then add the
                // occurence number onto the end of the name.
                int nameFrequency = nameFrequencies.ContainsKey(ourCar.Name)
                    ? nameFrequencies[ourCar.Name] : 0;
                carStates[i].name = ourCar.Name + ((nameFrequency == 0)
                    ? "" : " " + (nameFrequency + 1));
                nameFrequencies[ourCar.Name] = nameFrequency + 1;
            }
        }

        // Check if all cars have finished or have stalled.
        if (carStates != null) {
            endOfRaceObject.CheckRaceOver(carStates, raceMode);

            UpdateCarPositions();
        }
    }

    private void UpdateCarPositions() {
        for (int i = carsFinished; i < carStates.Length; i++) {
            carStates[i].position = GetPosition(carStates[i]);
        }
    }

    public void OnGUI() {
        CarState currentState = carStates[carManager.cameraManager.ActiveCamera];

        int y = 10;
        int yDiff = 30;

        Color carFinishedColor = Color.gray;
        Color carRacingColor = Color.white;
        Color carTimedOutColor = Color.red;

        // Draw the lap number for the current car.
        if (raceMode != RaceMode.Test) {
            GUI.Label(new Rect(10, y, 200, 100), "Lap " + (currentState.lap + 1), style);
            y += yDiff;
        }

        // Find the active camera to convert 3D coordinates to screen coordinates.
        Camera carCamera = currentState.car.transform.parent.GetComponentInChildren<Camera>();

        // Draw the place numbers.
        string[] ordinals = new string[] { "st", "nd", "rd", "th" };
        float nameLocation = float.MinValue;
        for (int i = 0; i < carStates.Length; i++) {
            float numberWidth = style.CalcSize(new GUIContent((i + 1).ToString())).x;
            Rect ordinalRect = new Rect(15 + numberWidth, y + yDiff * (i + 1), 200, 100);
            nameLocation = Math.Max(nameLocation, style.CalcSize(new GUIContent(ordinals[i])).x
                + ordinalRect.xMin);

            DrawOutlinedText(new Rect(13, y + yDiff * (i + 1), 200, 100), (i + 1).ToString(),
                Color.white, style);
            DrawOutlinedText(ordinalRect, ordinals[i], Color.white, ordinalStyle);
        }

        for (int i = 0; i < carStates.Length; i++) {
            int position = carStates[i].position + 1;
            Color textColor = (carStates[i].position < carsFinished)
                ? carFinishedColor
                : ((carStates[i].HasCarStopped) ? carTimedOutColor : carRacingColor);

            // Draw the car's name over its model if it's in front of the camera.
            Vector3 screenPosition = carCamera.WorldToScreenPoint(
                carStates[i].car.transform.position + 2.5f * Vector3.up);
            if (screenPosition.z > 0) {

                // Use 'minSize' as the font size if the car is further than 'farZ' from the
                // camera. If the car is closer than 'nearZ' use 'maxSize', otherwise linearly
                // scale the font size between these points.
                int originalFontSize = style.fontSize;
                float maxSize = originalFontSize;
                float minSize = 10;
                float nearZ = 5;
                float farZ = 20;
                style.fontSize = (int)Mathf.Clamp((minSize - maxSize) / (farZ - nearZ)
                    * (screenPosition.z - nearZ) + maxSize, minSize, maxSize);

                float xOffset = style.CalcSize(new GUIContent(carStates[i].name)).x / 2;
                DrawOutlinedText(new Rect(screenPosition.x - xOffset,
                    carCamera.pixelHeight - screenPosition.y, 200, 100), carStates[i].name,
                    Color.white, style);
                style.fontSize = originalFontSize;

            // Draw the car's row on the leaderboard.
            DrawOutlinedText(new Rect(nameLocation + 2, y + yDiff * position, 200, 100),
                carStates[i].name, textColor, style);
            }
        }
        y += yDiff * carStates.Length;
    }

    private void DrawOutlinedText(Rect location, string text, Color textColor, GUIStyle style) {
        Color originalColor = style.normal.textColor;
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(location.xMin + 1, location.yMin + 1, location.width, location.height),
            text, style);
        GUI.Label(new Rect(location.xMin + 1, location.yMin - 1, location.width, location.height),
            text, style);
        GUI.Label(new Rect(location.xMin - 1, location.yMin + 1, location.width, location.height),
            text, style);
        GUI.Label(new Rect(location.xMin - 1, location.yMin - 1, location.width, location.height),
            text, style);

        style.normal.textColor = textColor;
        GUI.Label(location, text, style);
        style.normal.textColor = originalColor;
    }

    public void TriggerEnter(int checkpoint, Collider car) {
        int carIndex = GetCarIndexFromCollider(car);

        // Something bad happened, and a warning has already been logged, so just return.
        if (checkpoint == -1 || carIndex == -1) {
            return;
        }

        // If the car just completed the next stage of the lap.
        if (carStates[carIndex].stage == checkpoint) {
            carStates[carIndex].stage++;
            carStates[carIndex].ResetCheckpointTime();

            // If the car just completed a lap.
            if (carStates[carIndex].stage == checkpoints.Length) {
                carStates[carIndex].stage = 0;
                carStates[carIndex].lap++;

                // Update the WWW form if this car just finished the race.
                if (carStates[carIndex].lap == LAPS_IN_RACE) {
                    endOfRaceObject.Finish(carStates[carIndex].name);
                    carsFinished++;
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
        int position = carsFinished;
        foreach (CarState other in carStates) {
            // Increment the position each time we find a car which is in front
            // of the given car.
            if (other.car != car.car && other.position >= carsFinished && !IsCarInFront(car, other)) {
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

        private Queue<string> carsFinishedQueue = new Queue<string>();
        private HashSet<string> finishedCars = new HashSet<string>();

        private bool hasSentObject = false;

        public EndOfRaceObject(MonoBehaviour outerClass) {
            this.outerClass = outerClass;
        }

        public void Finish(string scriptName) {
            if (!finishedCars.Contains(scriptName)) {
                carsFinishedQueue.Enqueue(scriptName);
                finishedCars.Add(scriptName);

                Debug.Log(scriptName + " has finished the race!");
            }
        }

        public bool CheckRaceOver(CarState[] states, RaceMode mode) {
            if (hasSentObject) {
                return true;
            }

            // Test mode is an endless race.
            if (mode == RaceMode.Test) {
                return false;
            }

            // The race is over only if all cars have finished the race or have
            // stopped, either because of a bad script or if the car has flipped over.
            bool isRaceOver = true;
            foreach (CarState car in states) {
                isRaceOver &= finishedCars.Contains(car.name) || car.HasCarStopped;
            }

            if (isRaceOver) {
                Send(states, mode);
            }

            return isRaceOver;
        }

        private void Send(CarState[] states, RaceMode mode) {
            if (hasSentObject) {
                return;
            }

            hasSentObject = true; // This function may only be called once.

            string[] carNames = new string[states.Length];
            int index = 0;

            Debug.Log("Race has finished!");

            // Add the cars which have finished the race to the top of the list.
            while (carsFinishedQueue.Count > 0) {
                Debug.Log("Finished with position " + index + ": " + carsFinishedQueue.Peek());
                carNames[index++] = carsFinishedQueue.Dequeue();
            }

            // Add the remaining cars.
            foreach (CarState car in states) {
                if (!finishedCars.Contains(car.name)) {
                    Debug.Log("Stalled with position " + car.position + ": " + car.name);
                    carNames[car.position] = car.name;
                }
            }

            // Finally, create the form.
            WWWForm form = new WWWForm();
            form.AddField("players", states.Length);
            for (int i = 0; i < states.Length; i++) {
                form.AddField(i.ToString(), carNames[i]);
            }

            outerClass.StartCoroutine(WaitForSend(form, mode));
        }

        private IEnumerator WaitForSend(WWWForm form, RaceMode mode) {
            string url = GetURL();
            string scoreUrl = url + "score";
            string leaderboardUrl = url + ((mode == RaceMode.Multiplayer)
                ? "leaderboard" : "tournament/next");

            // Send the the result of the race.
            Debug.Log("Sending \"" + System.Text.Encoding.Default.GetString(form.data)
                + "\" to '" + scoreUrl + "' ...");
            WWW www = new WWW(scoreUrl, form);

            // Wait until the form has been recieved.
            yield return www;

            if (www.error == null) {
                Debug.Log("End of race object sent! " + www.text);
            } else {
                Debug.LogError("Error sending: " + www.error);
            }

            Debug.Log("Redirecting to \"" + leaderboardUrl + "\" . . . ");
            Application.OpenURL(leaderboardUrl);
        }

        private string GetURL() {
            string url = Application.absoluteURL;
            string defaultUrl = "http://146.169.47.15:3026/"; // The url used when running locally.

            if (url.StartsWith("http://")) {
                url = url.Substring("http://".Length);
            }

            Regex regex = new Regex(".*(:\\d+)?/");
            Match match = regex.Match(url);
            if (match.Success) {
                return "http://" + match.Groups[0].Value;
            } else {
                Debug.LogWarning("Error finding the url. Using default url: \"" + defaultUrl + "\".");
                return defaultUrl;
            }
        }
    }
}
