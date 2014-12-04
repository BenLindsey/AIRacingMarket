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
              + ', "GetTimeToNextBoost":' + api.GetTimeToNextBoost()
              + ', "CarInFront":' + (api.CarInFront() ? "true" : "false")
              + ', "CarOnRight":' + (api.CarOnRight() ? "true" : "false") 
              + ', "CarOnLeft":'  + (api.CarOnLeft() ? "true" : "false")
              + ', "GetDistanceToNextCorner":' + api.GetDistanceToNextCorner()
              + ', "GetNextCornerAmount":' + api.GetNextCornerAmount() 
              + '}';

    //Debug.Log("Requesting browser to build commands");
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
    // Debug.Log("Executing built commands");
    eval(commands);
}
