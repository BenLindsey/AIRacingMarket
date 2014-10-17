using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class StoryScript : MonoBehaviour
{
	public abstract SortedDictionary<float, Pair<string,float>> getLocationText();
	public abstract SortedDictionary<float, Pair<string,float>> getTimeText();
}

public class Pair<T,U> {
	public T first;
	public U second;
		
	public Pair(T first, U second){
		this.first = first;
		this.second = second;		
	}
}
