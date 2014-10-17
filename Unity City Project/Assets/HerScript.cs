using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerScript : StoryScript {

	// Use this for initialization
	void Start () {
	
	}
	
	public override SortedDictionary<float, Pair<string,float>> getLocationText() {
		
		SortedDictionary<float, Pair<string,float>> locationText = new SortedDictionary<float, Pair<string,float>>();
			
		return locationText;
	}
	
	public override SortedDictionary<float, Pair<string,float>> getTimeText() {
		SortedDictionary<float, Pair<string,float>> timeText = new SortedDictionary<float, Pair<string,float>>();
		
		timeText.Add(1.5f, new Pair<string, float>("Blah blah blah",2.5f));
		timeText.Add(2.8f, new Pair<string, float>("TEST", 2.5f));
		
		return timeText;
	}
}
