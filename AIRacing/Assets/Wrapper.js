#pragma strict

var api : AiApi;

var scriptName;
var car;

function Start () {
}

function FixedUpdate () {   
    Application.ExternalCall("BuildCommands", car);
}

function SetScriptName(name) {
    scriptName = name;
}

function SetScriptContent(contents) {
}

function SetCar(carNo) {
    car = carNo;
}

function ExecuteCommands(commands) {
    eval(commands);
}
