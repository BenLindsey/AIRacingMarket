using UnityEngine;
using System.Collections;

public class LocalRaceStarter : MonoBehaviour {

	public string[] scriptsToRace;

	// Use this for initialization
	void Start () {
		foreach (string script in scriptsToRace) {
			GetComponent<CarManager>().AddLocalScript(script);
		}
		GetComponent<CarManager>().StartRace();
	}
}
