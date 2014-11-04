using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.ExternalCall("RequestLevel");
	}

    public void SetLevel(string levelName) {
        Debug.Log("Changing level to " + levelName);
        Application.LoadLevel(levelName);
    }
}
