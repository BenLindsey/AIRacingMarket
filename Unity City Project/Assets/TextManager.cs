using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour {
	
	public Thought thought;
	public StoryScript script;
	
	private SortedDictionary<float,Pair<string,float>> locationText;
	private SortedDictionary<float,Pair<string,float>> timeText;
	
	public float startZ, startTime = 0; 
	
	// Use this for initialization
	void Start () {
		
		locationText = script.getLocationText();
		timeText = script.getTimeText();

		startZ = transform.localPosition.z;
	}
	
	// Update is called once per frame
	void Update () {
		
		float myZ = transform.localPosition.z - startZ;
		float myTime = Time.time - startTime;
		
		if(startTime >= 0 ) {
			TimeText(myTime);
		}
		
		LocationText(myZ);
	}
	
	void LocationText(float zLocation) {
		foreach(KeyValuePair<float, Pair<string, float>> entry in locationText) {
			if(entry.Key < zLocation) {
				addTextToWorld(entry.Value.first,entry.Value.second);
				locationText.Remove(entry.Key);
			}
		}
	}
	
	void TimeText(float time) {
		foreach(KeyValuePair<float, Pair<string, float>> entry in timeText) {
			if(entry.Key < time) {
				addTextToWorld(entry.Value.first, entry.Value.second);
				timeText.Remove(entry.Key);
			}
		}
	}
	
	void addTextToWorld(string text, float time) {
		Thought newThought = Instantiate(thought,transform.localPosition,Quaternion.identity) as Thought;
		newThought.GetComponentInChildren<TextMesh>().text = text;
		newThought.time = time;
	}
}
