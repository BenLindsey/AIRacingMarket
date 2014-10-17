using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : StoryScript {
	
	void Start () {
	
	}
	
	public override SortedDictionary<float, Pair<string,float>> getLocationText() {
		
		SortedDictionary<float, Pair<string,float>> locationText = new SortedDictionary<float, Pair<string,float>>();
		
		locationText.Add(30f, new Pair<string, float>("Why is it always so busy \n around here?", 10f));
		locationText.Add(50f, new Pair<string, float>("Don't people have anything \n better to do?", 10f));
			
		return locationText;
	}
	
	public override SortedDictionary<float, Pair<string,float>> getTimeText() {
		SortedDictionary<float, Pair<string,float>> timeText = new SortedDictionary<float, Pair<string,float>>();
		
		timeText.Add(0f, new Pair<string, float>("Finally, some air.",2.5f));
		timeText.Add(2.5f, new Pair<string, float>("It's good to get out.", 2.5f));
		timeText.Add(5f, new Pair<string, float>("Use wasd to take a walk...",8f));
		
		return timeText;
	}
}
