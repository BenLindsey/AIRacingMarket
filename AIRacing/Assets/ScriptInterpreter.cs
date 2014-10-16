using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class ScriptInterpreter : MonoBehaviour  {

    public Car car;

    const string URL = "http://http://146.169.47.15:3000/";

    string scriptName;
    string scriptPath;

    bool fileSet;

    StreamReader sr;

    string[] script;
    int nextLine;

    WWW www;

    float wakeUpTime;

	// Use this for initialization
	void Start () {
        Debug.Log("Interpreter Started");
        //Application.ExternalCall("SetScript", "");
	}

    public void SetScriptName(string scriptName) {

        this.scriptName = scriptName;

        Debug.Log("Script name set: " + scriptName);

        string scriptPath = URL + "/script/" + scriptName;
        Debug.Log("Path set: " + scriptPath);
        
        www = new WWW(scriptPath);
    }

    public void SetLocalScriptPath(string scriptPath) {
        scriptPath = "file:///" + scriptPath;
        www = new WWW(scriptPath);
        Debug.Log("Local path set: " + scriptPath);
    }

    void Update() {

        if (www == null) {
            return;
        }

        if (www.isDone) {
            script = www.text.Split('\n');
            fileSet = true;
        }

        if (!fileSet || nextLine >= script.Length || Time.time < wakeUpTime) {
            return;
        }

        string command = script[nextLine++];
        if (command == null) {
            return;
        }

        Debug.Log("Command: " + command);

        if (command.StartsWith("throttle")) {
            int throttle = int.Parse(command.Substring(9, command.Length - 9));
            Debug.Log("Set throttle:" + throttle);

            car.SetThrottle(throttle / 100.0f);
        }
        else if (command.StartsWith("steer")) {
            int steer = int.Parse(command.Substring(6, command.Length - 6));
            Debug.Log("Set steer:" + steer);

            car.SetSteer(steer / 100f);
        }
        else if (command.StartsWith("wait"))
        {
            float wait = float.Parse(command.Substring(5, command.Length - 5));
            Debug.Log("Wait:" + wait);

            wakeUpTime = Time.time + wait;
        }
        else {
            Debug.LogWarning("Ignoring unknown command.");
        }
    }


    bool timeSent = false;

    // On End Zone enter    
    void OnTriggerEnter(Collider other) {
        if (timeSent)
            return;

        StartCoroutine(SendTime(Time.time));
    }

    IEnumerator SendTime(float time) {
        // Create a Web Form
        var form = new WWWForm();
        form.AddField("scriptname", scriptName);
        form.AddField("time", time.ToString());
        form.AddField("username", "Unity King");

        WWW www = new WWW(URL + "time/", form);

        yield return www;

        timeSent = true;
    }
}