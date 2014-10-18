#pragma strict

var api : AiApi;

var URL = "http://146.169.47.15:3000/";

var scriptName;
var scriptPath;

var scriptContents;

var www : WWW;

function Start () {
	api = GetComponentInChildren(AiApi);
	
	Application.ExternalCall("SetScript", "");
}

function Update () {
	if (www != null && www.isDone) {
		eval(www.text);
	}
}

function SetScriptName(name) {

	scriptName = name;

	Debug.Log("Script name set: " + scriptName);

	scriptPath = URL + "script/" + scriptName;
	Debug.Log("Path set: " + scriptPath);

	www = new WWW(scriptPath);
}

function SetLocalScriptPath(scriptPath) {
	scriptPath = "file:///" + scriptPath;
	www = new WWW(scriptPath);
	Debug.Log("Local path set: " + scriptPath);
}