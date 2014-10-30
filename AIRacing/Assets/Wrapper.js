#pragma strict

public var scriptName;

private var scriptContents;
private var api : AiApi;

var running = false;

function Start () {
}

function FixedUpdate () {
	if (running && scriptContents != null) {
		eval(scriptContents);
	}
}

function SetScriptName(name) {
    scriptName = name;
}

function SetScriptContents(contents) {
    scriptContents = contents;
}