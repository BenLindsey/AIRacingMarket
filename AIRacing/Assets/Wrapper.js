#pragma strict

var api : AiApi;

var URL = "http://146.169.47.15:3001/";

var endZone : EndZone;

var scriptName;
var scriptPath;

var scriptContents;

var www : WWW;

var loaded = false;
var script;

function Start () {
	Application.ExternalCall("SetScript", "");
    if (endZone != null) {
        endZone.SetURL(URL);
    }
}

function FixedUpdate () {
    if (!loaded && www != null && www.isDone) {
        loaded = true;
        script = www.text;
        if (endZone != null) {
            endZone.SetStartTime(Time.time);
        }
    } 

	if (loaded) {
		eval(script);
	}
}

function SetScriptName(name) {
    endZone.SetScriptName(name);

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