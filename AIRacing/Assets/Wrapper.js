#pragma strict

var api : AiApi;

var scriptName;
var car;


function Start () {
}

function FixedUpdate () {   
    var state = '{ "car":' + car
              + ', "GetLane":'  + api.GetLane() 
              + ', "GetSpeed":' + api.GetSpeed()
              + ', "CarInFront":' + (api.CarInFront() ? "true" : "false")
              + ', "CarOnRight":' + (api.CarOnRight() ? "true" : "false") 
              + ', "CarOnLeft":'  + (api.CarOnLeft() ? "true" : "false")
              + ', "GetCornerDirection":' + api.GetCornerDirection() 
              + '}';

    Application.ExternalCall("BuildCommands", state);
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
