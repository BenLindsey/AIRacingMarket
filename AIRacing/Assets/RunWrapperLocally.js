#pragma strict
import System.IO;

public var file : String = "test.txt";

private var folder : String = Directory.GetCurrentDirectory() + "\\Assets\\Racing Scripts\\";
private var wrapper : Wrapper;

function Start () {
	wrapper = GetComponent(Wrapper);

	var path = folder + file;
	if (File.Exists(path)) {
	    Debug.Log("Setting local script path to: " + path);
	    wrapper.SetLocalScriptPath(path);
	}
	else {
	    Debug.Log("Path: '" + path + "' does not exist.");
	}
}

function Update () {

}