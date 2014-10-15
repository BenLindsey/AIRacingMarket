using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class ScriptInterpreter : MonoBehaviour  {

    public Car car;
    public string scriptPath;

    bool fileSet;

    StreamReader sr;

    float wakeUpTime;

	// Use this for initialization
	void Start () {

	}

    void SetScriptPath(string scriptPath) {
        try {
            sr = new StreamReader(scriptPath);
            fileSet = true;
        }
        catch (Exception e) {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }

    void Update() {

        if (!fileSet || Time.time < wakeUpTime) {
            return;
        }

        string command = sr.ReadLine();

        if (command == null) {
            return;
        }

        if (command.StartsWith("throttle")) {
            int throttle = int.Parse(command.Substring(9, command.Length - 9));
            Debug.Log("Set throttle:" + throttle);

            car.SetThrottle(throttle / 100.0f);
        }
        else if (command.StartsWith("wait")) {
            float wait = float.Parse(command.Substring(5, command.Length - 5));
            Debug.Log("Wait:" + wait);

            wakeUpTime = Time.time + wait;
        }
    }
}