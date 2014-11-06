#pragma strict

var api : AiApi;

var scriptName;

var startScript;
var updateScript;

var data : Hashtable;

function Start () {
    data = new Hashtable ();

    if (startScript) {
		eval(startScript);
	}
}

function FixedUpdate () {   
    if (updateScript) {
		eval(updateScript);
	}
}

function SetScriptName(name) {
    scriptName = name;

    Debug.Log("Script added with name: " + scriptName);
}

function SetScriptContent(contents) {
    Debug.Log("Added script: " + contents);
    startScript = contents + "Init();";
    updateScript = contents + "PhysicsUpdate();";
}