#pragma strict

var api : AiApi;

var scriptName;
var scriptContents;

function Start () {
}

function FixedUpdate () {
	if (scriptContents) {
		eval(scriptContents);
	}
}

function SetScriptName(name) {
    scriptName = name;
}

function SetScriptContents(contents) {
    Debug.Log("Added script: " + contents);
    scriptContents = contents;
}