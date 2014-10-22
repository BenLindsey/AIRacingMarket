using UnityEngine;
using System.Collections;

public class EndZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    //Warning!!! Hacky code!
    string URL;
    string scriptName;
    float startTime;

    public void SetURL(string url) {
        URL = url; 
    }

    public void SetScriptName(string scriptName) {
        this.scriptName = scriptName;
    }

    public void SetStartTime(float startTime) {
        this.startTime = startTime;
    }

    bool timeSent = false;

    // On End Zone enter    
    void OnTriggerEnter(Collider other) {
        if (!other.transform.parent.name.Contains("Car") || timeSent)
            return;

        timeSent = true;

        StartCoroutine(SendTime(Time.time - startTime));
    }

    IEnumerator SendTime(float time) {

        Debug.Log("Finished! With a time of: " + time);

        // Create a Web Form
        var form = new WWWForm();
        form.AddField("scriptname", scriptName);
        form.AddField("time", time.ToString());
        form.AddField("levelname", "Mr Bubbles");

        WWW www = new WWW(URL + "time/", form);

        yield return www;
    }
}
