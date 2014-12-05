var buildUpdate = function(script) {
  console.log("Building update");
  var When = {events : []};
  
  console.log("Building If");
  
  //IF executes the right chain if func & previous
  //E.g. When.EVENT.IF(func).ACTION -> Run ACTION if EVENT occurs & func returns true
  When.If = function (func) {
      this.events[this.events.length - 1]
          = function(previous, func) {
              return function(api) {
                  if(func(api)) { return previous(api); }
              };
      }(this.events[this.events.length - 1], func);
      
      return this;
  };
  
  //WHILE executes the right chain if func has stayed true since left chain true
  //E.g. When.EVENT.WHILE(func).ACTION -> Run ACTION if EVENT occurs & func true
  //  OR EVENT occured previously and func has since always returned true
  When.While = function (func) {
      this.events[this.events.length - 1]
          = function(previous, func) {
              var triggered = false;
              return function(api) {
              	if(triggered) {
              	    previous(api);
              	    triggered = func(api);
              	} else {
                      triggered = previous(api) && func(api);
              	}
              	
              	return triggered;
              };
      }(this.events[this.events.length - 1], func);
      
      return this;
  };
  
  //When pauses the right chain until func returns true 
  When.When = function (func) {
      this.events[this.events.length - 1]
          = function(previous, func) {
              var triggered = false;
              return function(api) {
              	var previous_res = previous(api);
              	if((previous_res || triggered) && func(api)) {
              	    triggered = false
              	    return true;
              	} else {
                      triggered = triggered || previous_res;
                      return false;
              	}
              };
      }(this.events[this.events.length - 1], func);
      
      return this;
  };
  
  console.log("Building After");
  When.After = function (time, multipleTriggers) {
      this.events[this.events.length - 1]
          = function(previous, index, offset, multipleTriggers) {
              var triggerTimes = [];
              var currentTime = 1;
              
              return function(api) {      
              	currentTime++;
                  if(previous(api) 
                  && (multipleTriggers === true  
                      || triggerTimes.length == 0)) {
                      triggerTimes.push(currentTime + offset);
                  }
                  
                  if(triggerTimes.length > 0 
                  && triggerTimes[0] <= currentTime) {
                      triggerTimes.pop();
                      return true;
                  } else {
                      return false;
                  }
              };
      }(this.events[this.events.length - 1], 
        this.events.length - 1, time, multipleTriggers);   
      
      return this;
  };
  
  When.Not = function () {
      this.invert = true;
      return this;
  }
  
  When.Then = function (func) {
      this.events[this.events.length - 1]
          = function(previous, func) {
              return function(api) {
                  if(previous(api)) {
                      func(api);
                      return true;
                  } else {
                      return false
                  }
              };
      }(this.events[this.events.length - 1], func);
      
      return this;
  };
  
  When.And = function() {
      this.and = true;
      return this;
  };
  
  When.Or = function() {
      this.or = true;
      return this;
  };
  
  var Events = {
      "CarOnRight" : function(api) { return api.CarOnRight(); },
      "CarOnLeft" : function(api) { return api.CarOnLeft(); },
      "CarInFront" : function(api) { return api.CarInFront(); },
      "RaceStarts" : function(api, arg, arg2, state) { var temp = state.raceStarts; state.raceStarts = false; return temp; },
      "SpeedLessThan" : function(api, arg) { return api.GetSpeed() < arg; },
      "SpeedMoreThan" : function(api, arg) { return api.GetSpeed() > arg; },
      "NextCornerLeft" : function(api) { return api.GetNextCornerAmount() < 0; },
      "NextCornerRight" : function(api) { return api.GetNextCornerAmount() > 0; },
      "NextCornerDistanceBetween" : function(api, arg, arg2) { return api.GetDistanceToNextCorner() >= arg
                                                                   && api.GetDistanceToNextCorner() <= arg2; },
      "NextCornerDistanceLessThan" : function(api, arg) { return api.GetDistanceToNextCorner() < arg; },
      "NextCornerDistanceMoreThan" : function(api, arg) { return api.GetDistanceToNextCorner() > arg; },
  };
  
  //Build events TODO Add api gets
  console.log("Building events");
  for(var event in Events) {
      console.log("Setting event: " + event);
      When[event] = function(eventChecker) {
      	  var state = { raceStarts : true };
      	  
          return function(arg, arg2) {
              var func = function() {};

              if(this.invert) {  	
  	        func = function(api) {
  	            return !eventChecker(api, arg, arg2, state);
  	        };
              } else {
              	func = function(api) {
              	    return eventChecker(api, arg, arg2, state);
  	        });
              }
              
              if(this.and) {
              	this.events[this.events.length - 1]
          	  = function(previous, f) {
	              return function(api) {
	                return previous(api) && f(api);
	              };
      		    }(this.events[this.events.length - 1], func);
              } else if(this.or) {
                this.events[this.events.length - 1]
          	  = function(previous, f) {
	              return function(api) {
	                return previous(api) || f(api);
	              };
      		    }(this.events[this.events.length - 1], func);
              } else {
              	this.events.push(func);
              }
              
              this.and = false;
              this.or = false;
              this.invert = false;
              return this;
          };
      }(Events[event]);
  }
  
  //Build actions
  console.log("Building actions");
  var actions = ["SetSteer", "SetBrake", "SetThrottle", 
  	       "ChangeLaneRight",  "ChangeLaneLeft", 
  	       "SteerToMiddle", "SteerToLeft", "SteerToRight", "Boost"];
  for(var j = 0; j < actions.length; j++) {
      When[actions[j]] = function(action) {
          return function(arg) {
              this.events[this.events.length - 1]
                  = function(previous) {
                      return function(api) {
                          if(previous(api)) { 
                              api[action](arg); 
                              return true; 
                          } else { 
                              return false;   
                          }
                      };
              }(this.events[this.events.length - 1]);   
              
              return this;
          };
      }(actions[j]);
  }
		
	console.log("Evaling script " + script);
	// Execute the script, declaring globals as function variables
	eval(script);
	    
	console.log("Returning");
	// Use lambda to pass these variables as if globals
  return function(api) {
    PhysicsUpdate(api);
                		    	
    for(var eventI = 0; eventI < When.events.length; eventI++) {
      When.events[eventI](api);
    }
  };
};
