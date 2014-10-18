using UnityEngine;
using System.Collections;
using System.IO;

public class RunInterpreterLocally : MonoBehaviour {

    public ScriptInterpreter interpreter;
    public string file = "racetrack.txt";
    private string folder = Directory.GetCurrentDirectory() + "\\Assets\\Racing Scripts\\";

	// Use this for initialization
	void Start () {
        string path = folder + file;
        if (File.Exists(path)) {
            Debug.Log("Setting local script path to: " + path);
            interpreter.SetLocalScriptPath(path);
        }
        else {
            Debug.Log("Path: '" + path + "' does not exist.");
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
